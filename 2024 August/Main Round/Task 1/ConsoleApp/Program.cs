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
    /// Обработать один набор данных из входящего потока и вернуть ответ в виде объекта.
    /// </summary>
    static object ProcessDataSet(StreamReader input)
    {
        var number = input.ReadLine()!;

        // Если число содержит одну цифру, то в ответе получится 0.
        if (number.Length == 1)
            return "0";

        // находим позицию где очередная цифра меньше следующей цифры.
        var positionToRemove = number.Length - 1;
        for (var i = 0; i < number.Length - 1; i++)
        {
            if (number[i] < number[i + 1])
            {
                positionToRemove = i;
                break;
            }
        }

        // удаляем цифру и возвращаем оставшуюся строку.
        return number.Remove(positionToRemove, 1);
    }
}
