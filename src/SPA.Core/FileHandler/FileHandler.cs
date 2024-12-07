using SPA.Core.Other;

namespace SPA.Core.FileHandler;

public static class FileHandler
{
    public static string[][] GetLines(string filePath)
    {
        return File.ReadLines(filePath)
            .Select(x => x.Trim().Split(','))
            .ToArray();
    }

    public static string[] ReadNodesName(string filePath)
    {
        return File.ReadLines(filePath)
            .First()
            .Split(',')
            .Select(x => x.Trim())
            .ToArray();
    }

    public static string GenerateFileWithRandomGraph(int nodesNumber, double noEdgeChances, string targetPath)
    {
        var nodeNames = LetterSequenceGenerator.GenerateSequence(nodesNumber);
        var lines = new List<string[]> { nodeNames };

        for (var i = 0; i < nodesNumber; i++)
        {
            var line = new string[nodesNumber];
            for (var j = 0; j < nodesNumber; j++)
            {
                line[j] = i == j || Random.Shared.NextDouble() < noEdgeChances
                    ? "0"
                    : Random.Shared.Next(100, 1000).ToString();
            }
            lines.Add(line);
        }

        var fileName = $"graph_{nodesNumber}.txt";
        var filePath = Path.Combine(targetPath, fileName);
        File.WriteAllLines(filePath, lines.Select(x => string.Join(", ", x)));
        return filePath;
    }
}
