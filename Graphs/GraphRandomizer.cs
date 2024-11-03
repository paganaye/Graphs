namespace Graphs;

using System;

public static class GraphRandomizer
{
    private static Random _random = new Random();

    // Remplit la matrice d'adjacence d'un graphe avec des valeurs aléatoires 0 ou 1
    public static Graph RandomGraph(int nodeCount, double fill = 0.5)
    {
        GraphBuilder newGraph = new(nodeCount);
        for (int j = 1; j < nodeCount; j++)
        {
            for (int i = 0; i < j; i++)
            {
                bool isConnected = _random.NextDouble() < fill;
                if (isConnected) newGraph.SetEdge(i, j, true);
            }
        }

        return newGraph.Build();
    }
}