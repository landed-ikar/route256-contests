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
        // чтение кода дерева из входных данных.
        var treeCodeLength = int.Parse(input.ReadLine()!);
        var treeCodeStrings = input.ReadLine()!.Split(' ');
        var treeCode = new int[treeCodeLength];
        for (var i = 0; i < treeCodeLength; i++)
            treeCode[i] = int.Parse(treeCodeStrings[i]);

        // множество всех узлов дерева.
        var allNodes = new HashSet<int>();
        // множество только дочерних узлов дерева.
        var childNodes = new HashSet<int>();

        var treeCodeIndex = 0;
        while (treeCodeIndex < treeCodeLength)
        {
            // заполнение множества всех узлов дерева.
            var nodeNumber = treeCode[treeCodeIndex++];
            allNodes.Add(nodeNumber);

            // заполнение множества только дочерних узлов дерева.
            var childCount = treeCode[treeCodeIndex++];
            if (childCount != 0)
            {
                var currentChildNodes = treeCode.Skip(treeCodeIndex).Take(childCount);
                childNodes.UnionWith(currentChildNodes);
                treeCodeIndex += childCount;
            }
        }

        // ответом является единственный узел полученный вычитанием множеств всех узлов и дочерних узлов.
        return allNodes.Except(childNodes).Single();
    }
}
