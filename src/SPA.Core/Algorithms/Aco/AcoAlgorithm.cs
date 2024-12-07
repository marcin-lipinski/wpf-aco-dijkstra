using SPA.Core.Configuration;
using SPA.Core.GraphMath;
using SPA.Core.Interfaces;
using SPA.Core.Other;
using System.Diagnostics;

namespace SPA.Core.Algorithms.Aco;

internal class AcoAlgorithm : Algorithm
{
    private List<Ant> Ants { get; set; } = [];
    private List<Ant> BestAnts { get; set; } = [];

    public AcoAlgorithm(Graph graph, AlgorithmsConfig config, IResultLogger resultLogging, CancellationToken cancellationToken)
        :base(graph, config, resultLogging, cancellationToken)
    {
        InitializePheromones();
    }

    protected override void Run()
    {
        var stopwatch = new Stopwatch();
        var searchedPaths = -1;
        Logger.Log("ACO algorithm run...");
        for (int i = 0; i < Config.ShortestPathsNumber; i++)
        {
            if (searchedPaths != i)
            {
                Logger.Log($"Searching for path no. {i + 1}... ");
                searchedPaths = i;
                stopwatch.Restart();
            }

            Ant? bestAnt = null;
            for (int iter = 0; iter < Config.AcoOptions.IterationsNumber; iter++)
            {
                InitializeAnts();

                for (var j = 0; j < Graph.Nodes.Count - 1; j++)
                {
                    foreach (var ant in Ants)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        MoveAnt(ant);
                    }
                }
                UpdatePheronomones();

                var currentBestAnt = GetBestAnt();
                if (currentBestAnt != null && (bestAnt == null || currentBestAnt.TravalledDistance < bestAnt.TravalledDistance))
                {
                    bestAnt = currentBestAnt;
                }
            }

            if (bestAnt != null)
            {
                stopwatch.Stop();
                BestAnts.Add(bestAnt);
                RemovePathFromGraph(bestAnt);
                ShortestPaths.Add(new ShortestPath { Path = bestAnt.TravelledEdges.ToList(), Time = stopwatch.Elapsed });
                Logger.Log(Helper.PathToString(i + 1, bestAnt.TravelledEdges.ToList(), stopwatch.Elapsed));
            }
            else
            {
                i--;
            }
        }
    }

    private void RemovePathFromGraph(Ant ant)
    {
        foreach (var edge in ant.TravelledEdges)
        {
            edge.NodeA.Edges.Remove(edge);
        }
    }

    private void InitializePheromones()
    {
        foreach (var node in Graph.Nodes.Values)
        {
            foreach (var edge in node.Edges)
            {
                edge.Pheronomone = 1;
            }
        }
    }

    private void InitializeAnts()
    {
        var antsNumber = (int)Math.Round(Graph.Nodes.Count * Config.AcoOptions.AntsNumberRatio);
        Ants = Enumerable.Range(0, antsNumber).Select(x => new Ant
        {
            VisitedNodes = [Start],
            TravelledEdges = []
        }).ToList();
    }

    private void UpdatePheronomones()
    {
        foreach (var node in Graph.Nodes.Values)
        {
            foreach (var edge in node.Edges)
            {
                edge.Pheronomone *= Config.AcoOptions.EvaporationRate;
            }
        }

        foreach (var ant in Ants)
        {
            foreach (var edge in ant.TravelledEdges)
            {
                edge.Pheronomone += 1 / ant.TravalledDistance;
            }
        }
    }

    private void MoveAnt(Ant ant)
    {
        if (IsPathCompleted(ant)) return;

        var nextNode = Random.Shared.NextDouble() < Config.AcoOptions.RandomNodeFactor
                ? MoveToRandomNode(ant)
                : MoveToProbabilisticNode(ant);

        if (nextNode != null)
        {
            ant.VisitedNodes.Add(nextNode);
        }
    }

    private Node? MoveToRandomNode(Ant ant)
    {
        var possibleEdges = ant.VisitedNodes.Last().Edges
            .Where(edge => !ant.VisitedNodes.Contains(edge.NodeB))
            .ToList();

        if (!possibleEdges.Any()) return null;

        var choosenEdge = possibleEdges[Random.Shared.Next(0, possibleEdges.Count)];
        ant.TravelledEdges.Add(choosenEdge);
        return choosenEdge.NodeB;
    }

    private Node? MoveToProbabilisticNode(Ant ant)
    {
        var possibleEdges = ant.VisitedNodes.Last().Edges
            .Where(edge => !ant.VisitedNodes.Contains(edge.NodeB))
            .ToList();
        if (!possibleEdges.Any()) return null;

        var possibleProbabilities = new List<double>();
        var totalProbability = 0.0;

        foreach (var edge in possibleEdges)
        {
            var pheromoneOnPath = Math.Pow(edge.Pheronomone, Config.AcoOptions.Alpha);
            var heuristicForPath = Math.Pow(1 / edge.Wage, Config.AcoOptions.Beta);
            var probability = pheromoneOnPath * heuristicForPath;
            possibleProbabilities.Add(probability);
            totalProbability += probability;
        }

        if (totalProbability > 0.0)
        {
            for (var i = 0; i < possibleProbabilities.Count; i++)
            {
                possibleProbabilities[i] = possibleProbabilities[i] / totalProbability;
            }
        }

        var slices = new List<(Edge, double, double)>();
        var total = 0.0;
        for (var i = 0; i < possibleEdges.Count; i++)
        {
            slices.Add((possibleEdges[i], total, total + possibleProbabilities[i]));
            total += possibleProbabilities[i];
        }

        var spin = Random.Shared.NextDouble();

        var result = slices.Where(x => x.Item2 < spin && spin <= x.Item3);
        Edge choosenEdge;
        if (result.Any())
        {
            choosenEdge = result.First().Item1;
        }
        else choosenEdge = possibleEdges.First();

        ant.TravelledEdges.Add(choosenEdge);
        return choosenEdge.NodeB;
    }

    private Ant? GetBestAnt()
    {
        Ants.Sort(Ant.AntsComparison);
        return Ants.Where(IsPathCompleted).FirstOrDefault();
    }

    private bool IsPathCompleted(Ant ant)
    {
        return ant.VisitedNodes.Contains(Start) && ant.VisitedNodes.Contains(End);
    }
}
