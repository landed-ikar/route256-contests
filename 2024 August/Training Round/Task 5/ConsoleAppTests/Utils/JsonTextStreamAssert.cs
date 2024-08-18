using FluentAssertions;
using System.Text.Json;

namespace ConsoleAppTests.Utils;
public static class JsonTextStreamAssert
{
    static readonly JsonDocumentOptions _options = new JsonDocumentOptions { MaxDepth = 2048 };

    public static void Equal(Stream expectedStream, Stream actualStream)
    {
        expectedStream.Seek(0, SeekOrigin.Begin);
        actualStream.Seek(0, SeekOrigin.Begin);

        string expectedString, actualString;

        using (StreamReader reader = new StreamReader(expectedStream))
        {
            expectedString = reader.ReadToEnd().Replace("\r\n", "\n");
        }
        using (StreamReader reader = new StreamReader(actualStream))
        {
            actualString = reader.ReadToEnd().Replace("\r\n", "\n");
        }

        var expectedJsonNode = JsonDocument.Parse(expectedString, _options);
        var actualJsonNode = JsonDocument.Parse(actualString, _options);

        expectedJsonNode.Should().BeEquivalentTo(actualJsonNode, opt => opt.ComparingByMembers<JsonElement>());
    }
}
