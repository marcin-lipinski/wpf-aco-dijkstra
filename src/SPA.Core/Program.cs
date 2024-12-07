using SPA.Core.Algorithms;
using SPA.Core.Algorithms.Aco;
using SPA.Core.Algorithms.Dijkstra;
using SPA.Core.Configuration;
using SPA.Core.GraphMath;
using SPA.Core.Interfaces;
using System.Diagnostics;

namespace SPA.Core;

public class Program
{
    private readonly AlgorithmsConfig _config;
    private readonly IResultLogger _logger;

    public Program(IResultLogger logger, AlgorithmsConfig config)
    {
        _config = config;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Run(() =>
            {
                var dijkstraAlgorithm = new DijkstraAlgorithm(GenerateGraph(), _config, _logger, cancellationToken);
                var acoAlgorithm = new AcoAlgorithm(GenerateGraph(), _config, _logger, cancellationToken);

                var dijkstraResult = dijkstraAlgorithm.RunAlgorithmWithMeasures();
                var acoResult = acoAlgorithm.RunAlgorithmWithMeasures();

                var dijkstraPathSum = dijkstraResult.Sum(x => x.Path.Sum(y => y.Wage));
                var acoPathSum = acoResult.Sum(x => x.Path.Sum(y => y.Wage));
                var dijkstraTimeSum = TimeSpan.FromTicks(dijkstraResult.Sum(x => x.Time.Ticks));
                var acoTimeSum = TimeSpan.FromTicks(acoResult.Sum(x => x.Time.Ticks));

                var pathsQuality = Math.Abs((double)(dijkstraPathSum - acoPathSum) / (double)acoPathSum) * 100;
                var timeQuality = acoTimeSum - dijkstraTimeSum;

                _logger.Log($"Relative error for the paths wages sum: {pathsQuality.ToString("F2")} %.");
                _logger.Log($"Time difference: {timeQuality}.");
            }, cancellationToken);
        }
        catch
        {
            _logger.Throw($"Error: Unable to find {_config.ShortestPathsNumber} paths between node '{_config.StartNodeName}' and node '{_config.EndNodeName}'.");
        }
    }

    private Graph GenerateGraph()
    {
        try
        {
            var lines = FileHandler.FileHandler.GetLines(_config.GraphFilePath);
            return new Graph(lines);
        }
        catch
        {
            throw new Exception("Cannot generate graph from the file. Graph structure in the file is incorrect.");
        }
    }
}
