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
        long result = 0;

        //заполнение массива входных значений.
        var arrayLength = int.Parse(input.ReadLine()!);
        var stringParts = input.ReadLine()!.Split(' ');
        var array = new long[arrayLength];
        for (var i = 0; i < arrayLength; i++)
            array[i] = long.Parse(stringParts[i]);

        // создание и заполнение словаря с количеством разностей пар.
        var subtractionDict = new Dictionary<long, int>(arrayLength - 1);
        for (var i = 0; i < arrayLength - 1; i++)
        {
            var subtraction = array[i + 1] - array[i];
            if (subtractionDict.TryGetValue(subtraction, out var count))
            {
                subtractionDict[subtraction] = count + 1;
            }
            else
                subtractionDict[subtraction] = 1;
        }

        // подсчет количества пар чисел с такой же разностью
        for (var i = 0; i < arrayLength - 3; i++)
        {
            // вычисляем разницу в текущей паре
            var currentSubtraction = array[i + 1] - array[i];
            //вычисляем разницу в текущей паре пересекающейся с текущей парой (для дальнейшего исключения)
            var nextSubtraction = array[i + 2] - array[i + 1];

            // получаем количество пар с такой же разностью как и текущая пара чисел.
            var count = subtractionDict[currentSubtraction];
            // уменьшаем количество пар на 1 (исключаем текущую пару для последующих шагов).
            subtractionDict[currentSubtraction] = count - 1;

            result += currentSubtraction == nextSubtraction
                ? count - 2 // добавляем количество пар за исключением текущей пары и пары пересекающейся с текущей
                : count - 1; // добавляем количество пар за исключением текущей пары
        }
        return result;
    }
}
