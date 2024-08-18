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
        var results = ProcessDataSet(input);
        foreach (var result in results)
            output.WriteLine(result ? "1" : "0" );
    }

    /// <summary>
    /// Обработать один набор данных из входящего потока.
    /// </summary>
    static bool[] ProcessDataSet(StreamReader input)
    {
        // считываем логины существующих сотрудников и группируем их по символам в логине.
        var existingLoginsCount = ushort.Parse(input.ReadLine()!);
        var existingLoginDic = new Dictionary<string, List<string>>(existingLoginsCount);
        for (ushort i = 0; i < existingLoginsCount; i++)
        {
            var existingLogin = input.ReadLine()!;
            // ключ по которому группируются логины это отсортированный набор символов в логине.
            var key = String.Concat(existingLogin.Order());
            if (existingLoginDic.TryGetValue(key, out var logins))
                logins.Add(existingLogin);
            else
                existingLoginDic[key] = new List<string>() { existingLogin };
        }

        // считываем новые логины
        // сначала проверяем сесть ли существующие логины с таим же ключом
        // потом проверяем все логины с таким же ключом на соответствие.
        var newLoginsCount = ushort.Parse(input.ReadLine()!);
        var similarLogins = new bool[newLoginsCount];
        for (var i = 0; i < newLoginsCount; i++)
        {
            var newLogin = input.ReadLine()!;
            var key = String.Concat(newLogin.Order());

            if (existingLoginDic.TryGetValue(key, out var logins))
                similarLogins[i] = logins.Exists(x => IsLoginSimilar(newLogin, x));
            else
                similarLogins[i] = false;
        }

        return similarLogins;
    }

    /// <summary>
    /// Проверить посимвольно, являются ли логины похожими.
    /// </summary>
    static bool IsLoginSimilar(string login1, string login2)
    {
        // флаг показывающий что уже есть одна перестановка символов в логине
        var hasSwap = false;
        for (var i = 0; i < login1.Length; i++)
        {
            if (login1[i] == login2[i])
                continue;
            else if (!hasSwap && i != login1.Length -1 && login1[i] == login2[i + 1] && login1[i + 1] == login2[i])
            {
                hasSwap = true;
                i++;
                continue;
            }
            return false;
        }
        return true;
    }
}
