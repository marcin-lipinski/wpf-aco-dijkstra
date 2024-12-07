using SPA.Core.GraphMath;
using System.Text;

namespace SPA.Core.Other;

internal class Helper
{
    internal static string PathToString(int number, List<Edge> edges, TimeSpan elapsedTime)
    {
        var message = new StringBuilder($"Value: {edges.Sum(x => x.Wage)}\nTime: {elapsedTime.ToString()}\nPath: ");
        for (var p = 0; p < edges.Count; p++)
        {
            message.Append($"{edges[p].NodeA.Name} - ");
            if (p == edges.Count - 1)
            {
                message.Append($"{edges[p].NodeB.Name}");
            }
        }
        return message.ToString() + "\n";
    }
}
