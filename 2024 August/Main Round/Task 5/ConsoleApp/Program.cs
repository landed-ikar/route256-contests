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
        {
            ProcessDataSet(input, output);
            output.WriteLine();
        }
    }

    /// <summary>
    /// Обработать один набор данных из входящего потока.
    /// </summary>
    static void ProcessDataSet(StreamReader input, StreamWriter output)
    {
        var rowsCount = int.Parse(input.ReadLine()!);

        var path = new List<string>();
        var needWritePath = false;
        var isFirstRow = true;

        for (var i = 0; i < rowsCount; i++)
        {
            var rowArgs = input.ReadLine()!.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var key = rowArgs[0].Trim();
            var offset = (rowArgs[0].Length - key.Length) / 4;

            if (offset < path.Count)
            {
                path.RemoveRange(offset, path.Count - offset);
                needWritePath = true;
            }

            if (rowArgs.Length == 1)
            {
                path.Add(key);
                needWritePath = true;
            }
            else
            {
                var value = rowArgs[1].Trim();

                if (needWritePath)
                {
                    needWritePath = false;
                    if (!isFirstRow)
                        output.WriteLine();
                    if (path.Count > 0)
                        output.WriteLine("[{0}]", string.Join('.', path));
                }
                output.WriteLine("{0} = {1}", key, value);

                isFirstRow = false;
            }
        }
    }
}
