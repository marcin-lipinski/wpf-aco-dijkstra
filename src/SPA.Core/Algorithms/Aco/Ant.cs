using SPA.Core.GraphMath;

namespace SPA.Core.Algorithms.Aco;

internal class Ant
{
    public HashSet<Node> VisitedNodes { get; set; } = null!;
    public HashSet<Edge> TravelledEdges { get; set; } = null!;

    internal int TravalledDistance => TravelledEdges.Sum(x => x.Wage);

    internal static Comparison<Ant> AntsComparison = (a, b) =>
    {
        var distance = a.TravalledDistance - b.TravalledDistance;
        if (distance == 0)
        {
            return a.TravelledEdges.Count - b.TravelledEdges.Count;
        }
        return distance;
    };
}
