using SPA.Core.Configuration;
using SPA.Core.GraphMath;
using SPA.Core.Interfaces;
using System.Diagnostics;

namespace SPA.Core.Algorithms;

internal abstract class Algorithm
{
    protected readonly IResultLogger Logger;
    protected readonly CancellationToken CancellationToken;
    protected readonly Graph Graph;
    protected readonly Node Start;
    protected readonly Node End;
    protected readonly AlgorithmsConfig Config;
    protected readonly List<ShortestPath> ShortestPaths;

    protected Algorithm(Graph graph, AlgorithmsConfig config, IResultLogger logger, CancellationToken cancellationToken)
    {
        Logger = logger;
        CancellationToken = cancellationToken;
        Graph = graph;
        Start = graph.Nodes[config.StartNodeName!];
        End = graph.Nodes[config.EndNodeName!];
        Config = config;
        ShortestPaths = [];
    }

    public List<ShortestPath> RunAlgorithmWithMeasures()
    {
        GC.Collect();
        var memoryBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();

        Run();

        stopwatch.Stop();
        var memoryAfter = GC.GetAllocatedBytesForCurrentThread();
        Logger.Log($"Algorithm completed in {stopwatch.Elapsed.ToString()}");
        Logger.Log($"Algorithm memory usage: {(memoryAfter - memoryBefore) / 1024} KB");
        Logger.Log($"Algorithm paths wages sum: {ShortestPaths.Sum(x => x.Path.Sum(y => y.Wage))}\n");

        return ShortestPaths;
    }

    protected abstract void Run();
}
