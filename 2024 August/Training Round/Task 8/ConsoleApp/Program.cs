namespace ConsoleApp;
public static class Program
{
    public static void Main()
    {
        using var input = new StreamReader(Console.OpenStandardInput());
        using var output = new StreamWriter(Console.OpenStandardOutput());
        ProcessData(input, output);
    }

    /// <summary>
    /// Выполнить обработку данных из входящего потока и записать результат в выводящий поток.
    /// </summary>
    /// <remarks> Этот метод вынесен отдельно для удобства тестирования.</remarks>
    public static void ProcessData(StreamReader input, StreamWriter output)
    {
        // Получаем количество наборов данных и обрабатываем каждый набор.
        var dataSetCount = uint.Parse(input.ReadLine()!);
        for (var i = 0; i < dataSetCount; i++)
            output.WriteLine(ProcessDataSet(input));
    }

    /// <summary>
    /// Обработать один набор данных из входящего потока.
    /// </summary>
    static object ProcessDataSet(StreamReader input)
    {
        var townInfo = TownInfo.ReadTownInfo(input);
        var resourceTypeList = LoadResources(input);

        // сортируем ресурсы таким образом, чтобы редкие ресурсы шли сначала
        resourceTypeList.Sort((a, b) => a.Count.CompareTo(b.Count));

        var result = FindMinSquare(resourceTypeList, townInfo);

        return result;
    }

    static List<List<PointWithDistance>> LoadResources(StreamReader input)
    {
        var resourceTypeCount = int.Parse(input.ReadLine()!);
        var resourceTypeList = new List<List<PointWithDistance>>(resourceTypeCount);
        for (var i = 0; i < resourceTypeCount; i++)
        {
            var resourceCount = int.Parse(input.ReadLine()!);
            var resourceList = new List<PointWithDistance>(resourceCount);
            resourceTypeList.Add(resourceList);
            for (var j = 0; j < resourceCount; j++)
            {
                var resourcePositionInfos = input.ReadLine()!.Split(' ');
                var resourcePosition = new PointWithDistance(int.Parse(resourcePositionInfos[1]), int.Parse(resourcePositionInfos[0]));
                resourceList.Add(resourcePosition);
            }
        }
        return resourceTypeList;
    }

    /// <summary>
    /// Найти минимальную площадь области добычи.
    /// </summary>
    /// <param name="resources">Список координат ресурсов.</param>
    /// <param name="townInfo">Информация о городе.</param>
    /// <returns>Минимальная площадь области добычи.</returns>
    static int FindMinSquare(List<List<PointWithDistance>> resources, TownInfo townInfo)
    {
        var startArea = new ResourceArea(townInfo);
        var minArea = FindMinArea(resources, 0, startArea, startArea);
        return minArea.Square;
    }

    /// <summary>
    /// Найти минимально возможную область добычи ресурсов рекурсивно.
    /// </summary>
    /// <param name="resources">Список координат ресурсов.</param>
    /// <param name="resourceIndex">Индекс ресурса в списке координат ресурсов, для которого сейчас выполняется вычисление</param>
    /// <param name="currentArea">Область добычи на данный момент. (включает предыдущие ресурсы)</param>
    /// <param name="minArea">Минимальная область добычи рассчитанная на предыдущих итерациях.</param>
    /// <returns></returns>
    static ResourceArea FindMinArea(
        List<List<PointWithDistance>> resources, 
        int resourceIndex, 
        ResourceArea currentArea, 
        ResourceArea minArea)
    {
        // флаг того что ресурс уже находится в области добычи
        var inArea = false;
        // вычисляем дистанцию от области добычи до точек где есть ресурсы.
        foreach (var resource in resources[resourceIndex])
        {
            resource.Distance = currentArea.CalcDistance(resource);
            if (resource.Distance == 0)
            {
                inArea = true;
                break;
            }
        }

        // если ресурс уже находится в области добычи переходим к следующей итерации
        if (inArea)
        {
            if (resourceIndex == resources.Count - 1)
                return currentArea;
            else
                return FindMinArea(resources, resourceIndex + 1, currentArea, minArea);
        }

        // Сортируем координаты ресурса по расстоянию до области добычи
        resources[resourceIndex].Sort((a, b) => a.Distance.CompareTo(b.Distance));

        // Для каждой точки, начиная с самой ближней рекурсивно находим минимальную область добычи
        // и возвращаем ту область которая оказалась самой маленькой для всех точек.
        foreach (var point in resources[resourceIndex])
        {
            var nextArea = currentArea;
            if (!currentArea.IsPointInArea(point))
            {
                nextArea.Enlarge(point);
                if (nextArea.Square > minArea.Square)
                    continue;
            }

            if (resourceIndex == resources.Count - 1)
            {
                if (nextArea.Square < minArea.Square)
                    minArea = nextArea;
            }
            else
            {
                minArea = FindMinArea(resources, resourceIndex + 1, nextArea, minArea);
            }
        }
        return minArea;
    }

    /// <summary>
    /// Область добычи ресурсов.
    /// </summary>
    struct ResourceArea
    {
        /// <summary>
        /// Левый верхний угол.
        /// </summary>
        public PointWithDistance LeftTop { get; private set; }

        /// <summary>
        /// Правый нижний угол.
        /// </summary>
        public PointWithDistance RightBottom { get; private set; }

        /// <summary>
        /// Площадь.
        /// </summary>
        public int Square { get; private set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="townInfo"></param>
        public ResourceArea(TownInfo townInfo)
        {
            LeftTop = new PointWithDistance(townInfo.Width, townInfo.Height);
            RightBottom = new PointWithDistance(0, 0);
            Square = townInfo.Width * townInfo.Height;
        }

        /// <summary>
        /// Увеличить область добычи ресурсов, так чтобы она включала точку <paramref name="resourcePoint"/>.
        /// </summary>
        public void Enlarge(PointWithDistance resourcePoint)
        {
            LeftTop = new PointWithDistance(Math.Min(LeftTop.X, resourcePoint.X), Math.Min(LeftTop.Y, resourcePoint.Y));
            RightBottom = new PointWithDistance(Math.Max(RightBottom.X, resourcePoint.X), Math.Max(RightBottom.Y, resourcePoint.Y));

            var width = RightBottom.X - LeftTop.X + 1;
            var height = RightBottom.Y - LeftTop.Y + 1;
            Square = width * height;
        }

        /// <summary>
        /// Рассчитать расстояние от границ области добычи до точки.
        /// </summary>
        /// <param name="point">Точка, расстояние до которой надо рассчитать.</param>
        /// <returns>Сумма расстояния по оси X и Y. 0 если точка находится внутри области добычи.</returns>
        public int CalcDistance(PointWithDistance point)
        {
            var fromLeftX = Math.Max(0, LeftTop.X - point.X);
            var fromRightX = Math.Max(0, point.X - RightBottom.X);
            var fromLeftY = Math.Max(0, LeftTop.Y - point.Y);
            var fromRightY = Math.Max(0, point.Y - RightBottom.Y);

            var X = Math.Max(fromLeftX, fromRightX);
            var Y = Math.Max(fromLeftY, fromRightY);
            return X + Y;
        }

        /// <summary>
        /// Находится ли точка внутри области добычи.
        /// </summary>
        public bool IsPointInArea(PointWithDistance point)
        {
            return LeftTop.X <= point.X
                && RightBottom.X >= point.X
                && LeftTop.Y <= point.Y
                && RightBottom.Y >= point.Y;
        }
    }

    /// <summary>
    /// Информация о городе.
    /// </summary>
    struct TownInfo
    {
        /// <summary>
        /// Высота.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Ширина.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Прочитать информацию о городе из входных данных.
        /// </summary>
        public static TownInfo ReadTownInfo(StreamReader input)
        {
            var parts = input.ReadLine()!.Split(' ');
            var height = int.Parse(parts[0]);
            var width = int.Parse(parts[1]);
            return new TownInfo() { Height = height, Width = width };
        }
    }

    /// <summary>
    /// Точка с координатами и дистанцией.
    /// </summary>
    class PointWithDistance
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Distance { get; set; }

        public PointWithDistance(int x, int y)
        { X = x; Y = y; }
    }
}
