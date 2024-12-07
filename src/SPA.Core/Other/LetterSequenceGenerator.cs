namespace SPA.Core.Other;

public class LetterSequenceGenerator
{
    private static List<string> _results = new List<string>();

    public static string[] GenerateSequence(int number)
    {
        _results = [];
        int length = 1;
        int generatedCount = 0;
        var result = new List<string>();

        while (generatedCount < number)
        {
            foreach (var combination in GenerateCombinations(length))
            {
                result.Add(combination);
                generatedCount++;
                if (generatedCount >= number) break;
            }
            length++;
        }

        return result.ToArray();
    }

    private static IEnumerable<string> GenerateCombinations(int length)
    {
        char[] letters = new char[length];
        GenerateCombinationsRecursive(letters, length - 1);

        foreach (var result in _results)
        {
            yield return result;
        }

        _results.Clear();
    }

    private static void GenerateCombinationsRecursive(char[] letters, int position)
    {
        for (char c = 'A'; c <= 'Z'; c++)
        {
            letters[position] = c;
            if (position == 0)
            {
                _results.Add(new string(letters));
            }
            else
            {
                GenerateCombinationsRecursive(letters, position - 1);
            }
        }
    }
}