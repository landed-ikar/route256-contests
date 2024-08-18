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
        var idCount = int.Parse(input.ReadLine()!);
        var idStrings = input.ReadLine()!.Split(' ');
        var ids = new int[idCount];
        for (var i = 0; i < idCount; i++)
            ids[i] = int.Parse(idStrings[i]);

        return CalsMaxRequestLength(ids);
    }

    /// <summary>
    /// Рассчитать максимально возможную длину пар идентификаторов.
    /// </summary>
    /// <param name="ids">Массив идентификаторов.</param>
    static int CalsMaxRequestLength(int[] ids)
    {
        if (ids.Length < 3)
            return ids.Length;

        // Множество проверенных уникальных пар идентификаторов.
        // Здесь хранятся кортежи из двух идентификаторов.
        // Уникальность кортежа вычисляется по уникальности комбинаций его членов.
        var testedPairs = new HashSet<(int, int)>();
        var maxLength = 2;

        for (var i = 0; i < ids.Length - 1; i++)
        {
            // Формируем пару идентификаторов для проверки.
            // Пара формируется таким образом чтобы идентификаторы в ней шли в одном порядке.
            // это важно для вычисления уникальности пары во множестве проверенных пар.
            var id1 = ids[i];
            var id2 = ids[i + 1];
            var pair = (Math.Min(id1, id2), Math.Max(id1, id2));

            // пропускаем итерацию если эта пара уже проверена.
            if (testedPairs.Contains(pair))
                continue;

            // добавляем пару в множество проверенных пар.
            testedPairs.Add(pair);

            // рассчитываем максимальную длину для текущей пары
            var pairMaxLength = CalcPairMaxLength(ids, pair);

            //вычисляем максимальную длину для всех пар.
            maxLength = Math.Max(maxLength, pairMaxLength);
        }
        return maxLength;
    }

    /// <summary>
    /// Рассчитать максимальную длину пары идентификаторов.
    /// </summary>
    /// <param name="ids">Массив идентификаторов.</param>
    /// <param name="pair">Пара идентификаторов.</param>
    static int CalcPairMaxLength(int[] ids, (int, int) pair)
    {
        var maxLength = 0;
        var currentLength = 0;
        for (var i = 0; i < ids.Length; i++)
        {
            if (ids[i] == pair.Item1 || ids[i] == pair.Item2)
                currentLength++;
            else
            {
                maxLength = Math.Max(maxLength, currentLength);
                currentLength = 0;
            }
        }
        return Math.Max(maxLength, currentLength);
    }
}
