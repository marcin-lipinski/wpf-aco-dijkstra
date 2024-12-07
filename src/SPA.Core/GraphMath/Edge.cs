namespace SPA.Core.GraphMath;

internal class Edge
{
    public Node NodeA { get; set; } = null!;
    public Node NodeB { get; set; } = null!;
    public int Wage { get; set; }
    public double Pheronomone { get; set; }
}
