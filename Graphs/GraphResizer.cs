namespace  Graphs;
using System;

internal class GraphResizer
{

    public static void SetNodeCount(Graph graph, int newCount)
    {
        var clone = graph.Clone();
        graph.SetNodeCount(newCount);
        var minCount = Math.Min(clone.NodeCount, newCount);
        for (int j = 0; j < minCount; j++)
        {
            for (int i =0; i < j; i++)
            {
                if (clone.HasEdge(i, j))
                {
                    graph.SetEdge(i, j, true);
                }
            }
        }
    }
}