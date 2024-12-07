using SPA.Core.Algorithms.Aco;

namespace SPA.Core.Configuration;

public class AlgorithmsConfig
{
    public int ShortestPathsNumber { get; set; }
    public string StartNodeName { get; set; } = null!;
    public string EndNodeName { get; set; } = null!;
    public string GraphFilePath { get; set; } = null!;
    public AcoOptions AcoOptions { get; set; } = new();
}
