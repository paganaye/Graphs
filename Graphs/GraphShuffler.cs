namespace Graphs;

using System;
using System.Collections.Generic;
using System.Linq;

public static class GraphShuffler
{
    private static Random _random = new Random();

    public static Graph Shuffled(Graph graph, int? seed = null)
    {
        if (seed != null) _random = new Random(seed.Value);
        int nodeCount = graph.NodeCount;
        int[] previousPositions = new int[nodeCount];
        bool[,] previousEdges = new bool[nodeCount, nodeCount];

        // Initialize previousPositions and copy the edges
        for (int i = 0; i < nodeCount; i++)
        {
            previousPositions[i] = i;
            for (int j = 0; j < nodeCount; j++)
            {
                previousEdges[i, j] = graph.HasEdge(i, j);
            }
        }

        // Fisher-Yates shuffle
        for (int i = nodeCount - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (previousPositions[i], previousPositions[j]) = (previousPositions[j], previousPositions[i]);
        }

        // Rebuild the graph using the shuffled positions
        var newGraph = new GraphBuilder(nodeCount);
        for (int j = 1; j < nodeCount; j++)
        {
            for (int i = 0; i < j; i++)
            {
                newGraph.SetEdge(i, j,
                    previousEdges[previousPositions[i], previousPositions[j]]);
            }
        }

        return newGraph.Build();
    }
}

public static partial class GraphExtensions
{
    public static Graph shuffled(this Graph graph, int? seed = null)
    {
        return GraphShuffler.Shuffled(graph, seed);
    }
}