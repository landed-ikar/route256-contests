using Xunit;
using ConsoleApp;
using ConsoleAppTests.Utils;
using System.Text.RegularExpressions;

namespace Tests;

public class ProgramTests
{
    [Theory()]
    [ClassData(typeof(FileTestSource))]
    public void DoWorkTest(string testFileName)
    {
        using var inputStream = new FileStream(testFileName, FileMode.Open);
        using var input = new StreamReader(inputStream);
        using var outputStream = new MemoryStream();
        using var output = new StreamWriter(outputStream, leaveOpen: true);

        Program.ProcessData(input, output);

        output.Flush();
        using var expectedOutputStream = new FileStream($"{testFileName}.a", FileMode.Open);
        JsonTextStreamAssert.Equal(expectedOutputStream, outputStream);
    }
}
