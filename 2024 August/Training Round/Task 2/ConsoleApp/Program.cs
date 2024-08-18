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
        var dataSetInfo = input.ReadLine()!.Split(' ');
        var count = int.Parse(dataSetInfo[0]);
        var commission = decimal.Parse(dataSetInfo[1]) / 100;

        decimal result = 0;

        for (var i = 0; i < count; i++)
        {
            var cost = (decimal)int.Parse(input.ReadLine()!);
            // вычисление не округленной комиссии.
            var actualCommission = cost * commission;
            // вычисление правильной комиссии.
            var correctCommission = Math.Round(actualCommission, 2, MidpointRounding.ToZero);
            // вычисление ошибочной комиссии. Отличается от предыдущей строки количеством десятичных цифр.
            var incorrectCommission = Math.Round(actualCommission, 0, MidpointRounding.ToZero);
            result += correctCommission - incorrectCommission;
        }

        return result.ToString("f2");
    }
}
