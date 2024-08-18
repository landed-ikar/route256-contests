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
        var dataSetCount = uint.Parse(input.ReadLine()!);
        for (var i = 0; i < dataSetCount; i++)
            output.WriteLine(ProcessDataSet(input));
    }

    /// <summary>
    /// Обработать один набор данных из входящего потока.
    /// </summary>
    static object ProcessDataSet(StreamReader input)
    {
        // считываем размеры коробок
        int boxCount = int.Parse(input.ReadLine()!);
        var boxes = new List<Size>(boxCount);
        for (int i = 0; i < boxCount; i++)
            boxes.Add(Size.ReadSizeFromInput(input));

        // сортируем и убираем коробки которые полностью перекрываются другими коробками.
        boxes = FilterAndSortByDescendingHeight(boxes);

        // считываем размеры картин
        var pictureCount = int.Parse(input.ReadLine()!);
        var pictures = new List<Size>(pictureCount);
        for (var i = 0; i < pictureCount; i++)
            pictures.Add(Size.ReadSizeFromInput(input));

        // сортируем и убираем картины которые полностью перекрываются другими картинами.
        pictures = FilterAndSortByDescendingHeight(pictures);

        // Заполняем список где каждый элемент это список коробок
        // в которые может поместиться картина из отсортированного списка.
        var possibleBoxCombinations = GetPossibleBoxCombinations(boxes, pictures);
        if (possibleBoxCombinations == null)
            return -1;

        // Рассчитываем минимально возможную комбинацию коробок.
        return GetMinBoxesCount(possibleBoxCombinations);
    }

    /// <summary>
    /// Получить список возможных комбинаций коробок для упаковки картин.
    /// </summary>
    /// <param name="boxes">Отсортированный по высоте список размеров коробок.</param>
    /// <param name="pictures">Отсортированный по высоте список размеров картин.</param>
    /// <returns>Список возможных комбинаций коробок. null - если невозможно составить такой список.</returns>
    static List<List<Size>>? GetPossibleBoxCombinations(List<Size> boxes, List<Size> pictures)
    {
        var possibleBoxCombinations = new List<List<Size>>(pictures.Count);
        var indexOfFirstBoxThatCanContain = 0;
        foreach (var picture in pictures)
        {
            var possibleBoxes = new List<Size>();
            for (var i = indexOfFirstBoxThatCanContain; i < boxes.Count; i++)
            {
                if (boxes[i].CanContain(picture))
                {
                    if (possibleBoxes.Count == 0)
                        indexOfFirstBoxThatCanContain = i;
                    possibleBoxes.Add(boxes[i]);
                }
                else if (possibleBoxes.Count != 0)
                    break;
            }
            // не найдено коробок в которые может поместиться картина.
            if (possibleBoxes.Count == 0)
                return null;

            possibleBoxCombinations.Add(possibleBoxes);
        }
        return possibleBoxCombinations;
    }

    static int GetMinBoxesCount(List<List<Size>> combinations)
    {
        var minLength = combinations.Count;
        var combinationIndices = new int[combinations.Count];

        while (true)
        {
            var currentBoxCombination = new HashSet<Size>(combinations.Count);
            var i = 0;
            var isEarlyBreak = false;
            for (; i < combinations.Count; i++)
            {
                currentBoxCombination.Add(combinations[i][combinationIndices[i]]);
                if (currentBoxCombination.Count >= minLength)
                {
                    isEarlyBreak = true;
                    break;
                }
            }

            minLength = currentBoxCombination.Count;

            //берем следующую комбинацию индексов.
            var incrementIndex = isEarlyBreak ? i : combinations.Count - 1;
            while (true)
            {
                combinationIndices[incrementIndex]++;
                if (combinationIndices[incrementIndex] >= combinations[incrementIndex].Count)
                {
                    combinationIndices[incrementIndex] = 0;
                    if (incrementIndex == 0)
                        return minLength;
                    else
                        incrementIndex--;
                }
                else
                    break;
            }
        }
    }

    /// <summary>
    /// Отсортировать объекты по высоте и отфильтровать объекты которые полностью перекрываются другими объектами.
    /// </summary>
    /// <param name="list">Список размеров объектов.</param>
    /// <returns>Отсортированный и отфильтрованный список размеров объектов.</returns>
    static List<Size> FilterAndSortByDescendingHeight(List<Size> list)
    {
        list.Sort((a, b) => b.Height.CompareTo(a.Height));
        var result = new List<Size>(list.Count) { list[0] };
        for (var i = 1; i < list.Count; i++)
        {
            var resultIndex = result.Count - 1;
            // если высота следующего объекта равна высоте объекта в результате,
            // то оставляем объект у которого больше ширина.
            if (list[i].Height == result[resultIndex].Height && list[i].With > result[resultIndex].With)
            {
                result[resultIndex] = list[i];
            }
            // если ширина следующего объекта больше ширины объекта в результате,
            // то добавляем этот объект в результат. Иначе, следующий объект НЕ БОЛЬШЕ предыдущего
            // и его можно исключить из дальнейшей обработки.(не добавлять в результат)
            else if (list[i].With > result[resultIndex].With)
            {
                result.Add(list[i]);
            }
        }
        return result;
    }

    /// <summary>
    /// Размер объекта. Коробки или картины.
    /// </summary>
    struct Size
    {
        /// <summary>
        /// Ширина.
        /// </summary>
        public int With { get; private set; }

        /// <summary>
        /// Высота
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Считать размер объекта из входных данных.
        /// </summary>
        /// <param name="input">Входные данные.</param>
        /// <returns>Размер объекта.</returns>
        /// <remarks>
        /// По условиям задачи не важно какой размер является длинной, а какой шириной.
        /// Минимальная и максимальная стороны картины должны быть меньше либо равны 
        /// минимальной и максимальной сторонам коробки. Поэтому при считывании коробок и картин 
        /// мы их 'поворачиваем' так чтобы минимальная сторона всегда считалась высотой. (можно сделать и наоборот).
        /// </remarks>
        public static Size ReadSizeFromInput(StreamReader input)
        {
            var size = new Size();
            var parts = input.ReadLine()!.Split(' ');
            var a = int.Parse(parts[0]);
            var b = int.Parse(parts[1]);
            size.Height = Math.Min(a, b);
            size.With = Math.Max(a, b);
            return size;
        }

        /// <summary>
        /// Определяет может ли объект с одним размером полностью вместить в себя объект с другим размером размер.
        /// </summary>
        /// <param name="size">Размер который должен поместиться в объект с текущим размером.</param>
        public bool CanContain(Size size)
            => With >= size.With && Height >= size.Height;
    }
}
