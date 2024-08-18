using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text;

namespace ConsoleApp;
public static class Program
{
    /// <summary>
    /// Опции сериализации / десериализации JSON.
    /// </summary>
    static readonly JsonSerializerOptions _options = new JsonSerializerOptions { WriteIndented = false, MaxDepth = 2048 };

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
        var jsons = new JsonNode[dataSetCount];
        for (var i = 0; i < dataSetCount; i++)
            jsons[i] = ProcessDataSet(input);

        output.WriteLine(JsonSerializer.Serialize(jsons, _options));
    }

    /// <summary>
    /// Обработать один набор данных из входящего потока.
    /// </summary>
    static JsonNode ProcessDataSet(StreamReader input)
    {
        // считываем JSON строки из входных данных.
        var lineCount = ushort.Parse(input.ReadLine()!);
        var sb = new StringBuilder();
        for (ushort i = 0; i < lineCount; i++)
            sb.Append(input.ReadLine()!);

        // десериализуем JSON текст в JsonNode
        var jsonNode = JsonSerializer.Deserialize<JsonNode>(sb.ToString(), _options)!;
        
        PrettifyJsonNode(jsonNode);

        return jsonNode;
    }

    /// <summary>
    /// Удалить пустые словари и списки из JsonNode рекурсивно.
    /// </summary>
    static void PrettifyJsonNode(JsonNode jsonNode)
    {
        if (jsonNode is JsonArray array)
        {
            //список элементов для удаление
            var nodesToRemove = new List<JsonNode>();
            
            foreach (var value in array)
            {
                // выполняем операцию рекурсивно для каждого элемента.
                PrettifyJsonNode(value!);

                // если элемент пустой, добавляем его в список на удаление.
                if (IsEmpty(value!))
                    nodesToRemove.Add(value!);
            }

            // удаляем пустые элементы из списка.
            foreach (var node in nodesToRemove)
                array.Remove(node);
        }

        if (jsonNode is JsonObject obj)
        {
            // список ключей для удаления
            var namesToRemove = new List<string>();

            foreach ((var name, var value) in obj)
            {
                // выполняем операцию рекурсивно для каждого элемента.
                PrettifyJsonNode(value!);

                // если элемент пустой, добавляем его в список на удаление.
                if (IsEmpty(value!))
                    namesToRemove.Add(name);
            }

            // удаляем пустые элементы из словаря
            foreach (var name in namesToRemove)
                obj.Remove(name);
        }
    }

    /// <summary>
    /// Является ли JsonNode пустым.
    /// </summary>
    static bool IsEmpty(JsonNode jsonNode)
    {
        if (jsonNode is JsonArray array && array.Count == 0)
            return true;
        if (jsonNode is JsonObject obj && obj.Count == 0)
            return true;
        return false;
    }
}
