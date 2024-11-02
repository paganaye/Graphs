using System;
using Graphs;

internal class GraphCloner
{
    public static Graph CopyResizeGraph(Graph graph, int nodeCount)
    {
        // Create a new graph with the specified node count
        Graph newGraph = new Graph(nodeCount);

        // Determine the minimum number of nodes to copy (in case of downsizing)
        int minCount = Math.Min(graph.NodeCount, nodeCount);

        // Copy edges from the original graph to the new graph
        for (int i = 0; i < minCount; i++)
        {
            for (int j = i + 1; j < minCount; j++)
            {
                if (graph.HasEdge(i, j))
                {
                    newGraph.SetEdge(i, j, true);
                }
            }
        }

        return newGraph;
    }
}