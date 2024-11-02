using System;

namespace Graphs
{
    public static class GraphRandomizer
    {
        private static Random _random = new Random();

        // Remplit la matrice d'adjacence d'un graphe avec des valeurs aléatoires 0 ou 1
        public static void FillRandomAdjacencies(Graph graph)
        {
            int nodeCount = graph.NodeCount;

            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    bool isConnected = _random.Next(2) == 1;
                    graph.SetEdge(i, j, isConnected);
                }
            }
        }
    }
}