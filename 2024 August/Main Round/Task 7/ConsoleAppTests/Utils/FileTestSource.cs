using System.Collections;

namespace ConsoleAppTests.Utils;
public class FileTestSource : IEnumerable<object[]>
{
    const string TestDataFolder = "TestData";
    public IEnumerator<object[]> GetEnumerator()
    {
        var testFileNames = Directory.GetFiles(TestDataFolder)
            .Select(x => Path.Combine(TestDataFolder, Path.GetFileNameWithoutExtension(x)))
            .Distinct()
            .ToList();
        foreach (var testFileName in testFileNames)
            yield return new object[] { testFileName };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
