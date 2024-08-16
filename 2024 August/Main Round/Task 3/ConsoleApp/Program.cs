using System.Text;

namespace ConsoleApp;
public static class Program
{
    const string Message404 = "404";
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
            ProcessDataSet(input, output);
    }

    /// <summary>
    /// Обработать один набор данных из входящего потока.
    /// </summary>
    static void ProcessDataSet(StreamReader input, StreamWriter output)
    {
        var requestCount = int.Parse(input.ReadLine()!);

        var productsByIds = new Dictionary<int, string>();
        var idsByProduct = new Dictionary<string, int>();
        var history = new ProductHistoryRecord?[requestCount + 1];

        for (var currentTime = 1; currentTime < requestCount + 1; currentTime++)
        {
            var requestArgs = input.ReadLine()!.Split(' ');
            var command = requestArgs[0];
            if (command == "CHANGE")
            {
                var product = requestArgs[1];
                var id = int.Parse(requestArgs[2]);
                SetOrUpdateProductId(product, id, currentTime, history, productsByIds, idsByProduct);
            }
            else
            {
                var id = int.Parse(requestArgs[1]);
                var time = int.Parse(requestArgs[2]);
                var product = GetProductByIdAndTime(id, time, history);
                output.WriteLine(product);
            }
        }
    }

    /// <summary>
    /// Установить или обновить id у продукта.
    /// </summary>
    /// <param name="product">Название продукта.</param>
    /// <param name="id">Новый id продукта.</param>
    /// <param name="time">Текущее время.</param>
    /// <param name="history">История изменений продуктов.</param>
    /// <param name="productsById">Словарь продуктов по идентификаторам.</param>
    /// <param name="idsByProduct">Словарь идентификаторов по продуктам.</param>
    static void SetOrUpdateProductId( 
        string product, int id, int time,
        ProductHistoryRecord?[] history, Dictionary<int, string> productsById, Dictionary<string, int> idsByProduct)
    {
        // создаем новую запись для истории.
        var historyRecord = new ProductHistoryRecord()
        {
            Name = product,
            NewId = id,
        };
        // проверяем есть ли на данный момент у продукта другой id
        if (idsByProduct.TryGetValue(product, out var oldId) && oldId != id)
        {
            historyRecord.OldId = oldId;
            productsById.Remove(oldId);
        }
        // проверяем задан ли на данный момент id другому продукту
        if (productsById.TryGetValue(id, out var oldProduct) && oldProduct != product)
        {
            idsByProduct.Remove(oldProduct);
        }

        history[time] = historyRecord;
        idsByProduct[product] = id;
        productsById[id] = product;
    }

    /// <summary>
    /// Получить название продукта по id и времени.
    /// </summary>
    /// <param name="id">id продукта.</param>
    /// <param name="time">время.</param>
    /// <param name="history">История изменений продуктов.</param>
    /// <returns>Название продукта.</returns>
    static string GetProductByIdAndTime(int id, int time, ProductHistoryRecord?[] history)
    {
        for (var j = time; j >= 0; j--)
        {
            // в это время не было изменений в продуктах.
            if (!history[j].HasValue)
                continue;
            // продукту установили искомый id.
            if (history[j]!.Value.NewId == id)
                return history[j]!.Value.Name;
            // у продукта сбросили искомый id.
            if (history[j]!.Value.OldId == id)
                return Message404;
        }
        return Message404;
    }

    /// <summary>
    /// Запись в логе с изменениями в продуктах.
    /// </summary>
    struct ProductHistoryRecord
    {
        /// <summary>
        /// Предыдущий ID товара.
        /// </summary>
        public int? OldId;
        /// <summary>
        /// Новый ID товара.
        /// </summary>
        public int NewId;
        /// <summary>
        /// Имя товара.
        /// </summary>
        public string Name;
    }
}
