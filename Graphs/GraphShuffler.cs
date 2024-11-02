using System;
using System.Collections.Generic;
using System.Linq;

namespace Graphs
{
    public static class GraphShuffler
    {
        private static Random _random = new Random();

        public static void ShuffleGraph(Graph graph)
        {
            int nodeCount = graph.NodeCount;
            int[] previousPositions = new int[nodeCount];
            bool[,] previousEdges = new bool[nodeCount, nodeCount];

            // Initialize previousPositions to point to the current positions
            for (int i = 0; i < nodeCount; i++)
            {
                previousPositions[i] = i;
                for (int j = 0; j < nodeCount; j++)
                {
                    previousEdges[i, j] = graph.HasEdge(i, j);
                }
            }

            // Shuffle the previousPositions array
            previousPositions = previousPositions.OrderBy(x => _random.Next()).ToArray();

            // Rebuild the graph using the shuffled previous positions
            for (int j = 1; j < nodeCount; j++)
            {
                for (int i = 0; i < j; i++)
                {
                    graph.SetEdge(i, j, previousEdges[previousPositions[i], previousPositions[j]]); // Set edge in the new graph
                }
            }
        }
    }
}