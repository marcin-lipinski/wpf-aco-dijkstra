namespace SPA.Core.Algorithms.Aco;

public class AcoOptions
{
    public double AntsNumberRatio { get; set; } = 1;
    public int IterationsNumber { get; set; } = 1000;
    public double Alpha { get; set; } = 2;
    public double Beta { get; set; } = 4;
    public double RandomNodeFactor { get; set; } = 0.05;
    public double EvaporationRate { get; set; } = 0.5;
}
