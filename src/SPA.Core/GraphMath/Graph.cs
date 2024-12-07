namespace SPA.Core.GraphMath;

public class Graph
{
    internal Dictionary<string, Node> Nodes = [];

    public Graph(string[][] lines)
    {
        foreach (var item in lines)
        {
            for (var i = 0; i < item.Length; i++)
            {
                item[i] = item[i].Trim();
            }
        }

        if (lines.Skip(1).Any(x => x.Length != lines[0].Length) ||
            lines.Length != lines[0].Length + 1 ||
            lines.Skip(1).Any(x => x.Any(y => !int.TryParse(y, out _)))
        )
        {
            throw new Exception();
        }

        AddNodes(lines[0]);
        for (var i = 0; i < lines[0].Length; i++)
        {
            var currentNodeName = lines[0][i];
            var edges = lines.Skip(1).Select((line, ind) => new { Name = lines[0][ind], Wage = int.Parse(line[i]) });

            foreach (var edge in edges)
            {
                if (edge.Wage == 0) continue;
                AddEdge(currentNodeName, edge.Name, edge.Wage);
            }
        };
    }

    internal void AddEdge(string nodeAName, string nodeBName, int wage)
    {
        var edges = Nodes[nodeAName].Edges;

        edges.Add(new Edge
        {
            NodeA = Nodes[nodeAName],
            NodeB = Nodes[nodeBName],
            Wage = wage
        });
    }

    internal void AddNodes(string[] names)
    {
        foreach (var name in names)
        {
            Nodes.Add(name, new Node { Name = name });
        }
    }

    internal Node GetRandomNode()
    {
        return Nodes.Values.ToList()[Random.Shared.Next(0, Nodes.Values.Count)];
    }
}
