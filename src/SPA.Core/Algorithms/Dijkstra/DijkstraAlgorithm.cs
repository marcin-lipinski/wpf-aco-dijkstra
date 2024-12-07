using SPA.Core.Configuration;
using SPA.Core.GraphMath;
using SPA.Core.Interfaces;
using SPA.Core.Other;
using System.Diagnostics;

namespace SPA.Core.Algorithms.Dijkstra;

internal class DijkstraAlgorithm : Algorithm
{
    private readonly Dictionary<string, int> distances = [];
    private readonly Dictionary<string, Node?> previous = [];
    private readonly Dictionary<string, bool> visited = [];

    internal DijkstraAlgorithm(Graph graph, AlgorithmsConfig config, IResultLogger logger, CancellationToken cancellationToken)
        : base(graph, config, logger, cancellationToken)
    {
        InitializeArrays();
    }

    protected override void Run()
    {
        Logger.Log("Dijkstra algorithm run...");
        var stopwatch = new Stopwatch();
        for (var i = 0; i < Config.ShortestPathsNumber; i++)
        {
            Logger.Log($"Searching for path no. {i + 1}... ");
            stopwatch.Restart();
            InitializeArrays();
            var currentNode = Start;

            while (visited.Any(x => !x.Value))
            {
                foreach (var edge in currentNode.Edges)
                {
                    if (distances[edge.NodeB.Name] > distances[currentNode.Name] + edge.Wage)
                    {
                        distances[edge.NodeB.Name] = distances[currentNode.Name] + edge.Wage;
                        previous[edge.NodeB.Name] = currentNode;
                    }

                    if (CancellationToken.IsCancellationRequested) return;
                }
                var possibleKeys = visited.Where(x => x.Value == false).Select(x => x.Key);
                var nextNode = distances.Where(x => possibleKeys.Contains(x.Key)).OrderBy(x => x.Value).FirstOrDefault();
                currentNode = Graph.Nodes[nextNode.Key];
                visited[currentNode.Name] = true;
            }

            var path = new List<Edge>();
            var current = End;

            while (current != Start)
            {
                var prev = previous.FirstOrDefault(x => x.Key == current!.Name).Value;
                var edge = prev!.Edges.FirstOrDefault(y => y.NodeB == current)!;
                path.Add(edge);
                current = prev;
                current.Edges.Remove(edge);
            }
            path.Reverse();
            stopwatch.Stop();
            ShortestPaths.Add(new ShortestPath { Path = path, Time = stopwatch.Elapsed });

            Logger.Log(Helper.PathToString(i + 1, path, stopwatch.Elapsed));
        }
    }

    private void InitializeArrays()
    {
        distances.Clear();
        previous.Clear();
        visited.Clear();
        foreach (var node in Graph.Nodes)
        {
            distances.Add(node.Key, int.MaxValue);
            previous.Add(node.Key, null);
            visited.Add(node.Key, false);
        }
        distances[Start.Name] = 0;
        visited[Start.Name] = true;
    }
}
