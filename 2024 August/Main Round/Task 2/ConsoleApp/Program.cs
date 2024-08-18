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
        // считываем массивы из входного потока.
        var length = uint.Parse(input.ReadLine()!);
        var minArrayStrings = input.ReadLine()!.Split(' ');
        var maxArrayStrings = input.ReadLine()!.Split(' ');

        ulong result = 1;
        for (ulong i = 0; i < length; i++)
        {
            // число в массиве [1,2,...,n] используемого в качестве делителя
            var divider = i + 1;

            // считываем числа из массивов l и r.
            var minNumber = ulong.Parse(minArrayStrings[i]);
            var maxNumber = ulong.Parse(maxArrayStrings[i]);

            // находим количество возможных значений между l и r, которые делятся без остатка на делитель.
            var variantsFromMin = (minNumber - 1) / divider;
            var variantsFromMax = maxNumber / divider;
            var possibleVariants = variantsFromMax - variantsFromMin;

            // умножаем предыдущий результат на количество возможных вариантов и считает остаток.
            result = result * possibleVariants % 1_000_000_007;
        }

        return result;
    }
}
