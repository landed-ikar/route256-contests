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
        var length = int.Parse(input.ReadLine()!);

        var string1 = input.ReadLine()!;
        var string2 = input.ReadLine()!;

        // Если строки с исходным и сортированным массивом имеют разную длину,
        // значит какое-то из условий преобразования нарушается (например есть ведущие нули).
        if (string1.Length != string2.Length)
            return "no";

        // заполняем и сортируем исходный массив
        var arrayString1 = string1.Split(' ');
        var array1 = new long[length];
        for (var i = 0; i < length; i++)
        {
            array1[i] = int.Parse(arrayString1[i]);
        }
        Array.Sort(array1);

        // формируем строку с отсортированным массивом
        var sortedArrayString1 = string.Join(' ', array1);

        // определяем результат сравнением полученной строки и строки которую надо провалидировать.
        return sortedArrayString1.Equals(string2)
            ? "yes"
            : "no";
    }
}
