namespace Graphs;

using System;

internal static class GraphResizer
{
    public static Graph Resize(Graph graph, int newCount)
    {
        var clone = new GraphBuilder(newCount);
        var minCount = Math.Min(graph.NodeCount, newCount);
        for (int j = 0; j < minCount; j++)
        {
            for (int i = 0; i < j; i++)
            {
                if (graph.HasEdge(i, j))
                {
                    clone.SetEdge(i, j, true);
                }
            }
        }

        return clone.Build();
    }
}