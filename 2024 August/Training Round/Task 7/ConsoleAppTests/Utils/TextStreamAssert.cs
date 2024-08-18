using Xunit;
using Assert = Xunit.Assert;

namespace ConsoleAppTests.Utils;
public static class TextStreamAssert
{
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

        Assert.Equal(expectedString, actualString);
    }
}
