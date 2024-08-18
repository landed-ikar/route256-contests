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
        // Считываем информацию о машинах из входных данных.
        var carInfoStrings = input.ReadLine()!.Split(' ');
        var carCount = int.Parse(carInfoStrings[0]);
        var carCapacity = int.Parse(carInfoStrings[1]);

        // Считываем информацию о коробках.
        // причем храним информацию о коробках в виде словаря, где
        // ключ - вес коробки, значение - количество таких коробок.
        var boxCount = ushort.Parse(input.ReadLine()!);
        if (boxCount == 0)
            return 0;
        var boxWeights = new Dictionary<double, ushort>();
        var weightStrings = input.ReadLine()!.Split(' ');
        for (short i = 0; i < boxCount; i++)
        {
            var weight = Math.Pow(2, byte.Parse(weightStrings[i]));
            if (!boxWeights.ContainsKey(weight))
                boxWeights[weight] = 0;
            boxWeights[weight]++;
        }

        // формируем список уникальных весов коробок отсортированный по убыванию веса.
        var uniqueWeights = boxWeights.Keys.OrderDescending().ToList();
        var raceCount = 0;

        // пока есть коробки пытаемся взять самую большую, которая еще влезает в свободную машину
        // если такая коробка есть, удаляем ее из списка коробок и уменьшаем объем машины,
        // если такой коробки нет переходим к следующей машине.
        while (boxWeights.Count > 0)
        {
            raceCount++;
            for (var carIndex = 0; carIndex < carCount; carIndex++)
            {
                var restCapacity = (double)carCapacity;
                while (uniqueWeights.Exists(x => x <= restCapacity))
                {
                    var takenWeight = uniqueWeights.First(x => x <= restCapacity);
                    restCapacity -= takenWeight;
                    RemoveBox(takenWeight, boxWeights, uniqueWeights);
                }
            }
        }
        return raceCount;
    }

    /// <summary>
    /// Удалить коробку с определенным весом из словаря с весами коробок.
    /// </summary>
    /// <param name="weight">Вес коробки, которую удаляем.</param>
    /// <param name="boxWeights">Словарь с весами коробок</param>
    /// <param name="uniqueWeights">Список уникальных весов коробок.</param>
    static void RemoveBox(double weight, Dictionary<double, ushort> boxWeights, List<double> uniqueWeights)
    {
        if (boxWeights[weight] == 1)
        {
            boxWeights.Remove(weight);
            uniqueWeights.Remove(weight);
        }
        else
            boxWeights[weight]--;
    }
}
