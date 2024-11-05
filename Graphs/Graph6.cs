namespace Graphs;

using System;
using System.Text;

public static class Graph6
{
    public static string? Serialize(Graph graph)
    {
        int nodeCount = graph.NodeCount;

        if (nodeCount > 62)
            return null;

        StringBuilder sb = new StringBuilder();

        sb.Append((char)(nodeCount + 63));

        int edgeCount = nodeCount * (nodeCount - 1) / 2;
        int totalBits = edgeCount;
        int bitIndex = 0;
        int currentChar = 0;

        for (int j = 1; j < nodeCount ; j++)
        {
            for (int i = 0; i < j; i++)
            {
                if (graph.HasEdge(i, j))
                {
                    currentChar |= 1 << (5 - (bitIndex % 6));
                }

                bitIndex++;

                if (bitIndex % 6 == 0)
                {
                    sb.Append((char)(currentChar + 63));
                    currentChar = 0;
                }
            }
        }

        if (bitIndex % 6 != 0)
        {
            sb.Append((char)(currentChar + 63));
        }

        return sb.ToString();
    }

    public static Graph? Deserialize(string graph6)
    {
        if (string.IsNullOrEmpty(graph6)) return null;

        int index = 0;

        int c = graph6[index++];
        int nodeCount = c - 63;

        if (nodeCount > 62)
            return null;

        int edgeCount = nodeCount * (nodeCount - 1) / 2;
        int totalBits = edgeCount;
        int edgeBitsRead = 0;

        int i = 0;
        int j = 1;
        var graph = new GraphBuilder(nodeCount);

        while (edgeBitsRead < totalBits && index < graph6.Length)
        {
            int cEdge = graph6[index++] - 63;

            for (int bit = 5; bit >= 0; bit--)
            {
                if (edgeBitsRead >= totalBits)
                    break;

                bool hasEdge = ((cEdge >> bit) & 1) == 1;
                // Set the edge in the adjacency matrix
                graph.SetEdge(i, j, hasEdge);
                edgeBitsRead++;

                // Move to the next edge in order
                i++;
                if (i == j)
                {
                    i = 0;
                    j++;
                }

                if (j == nodeCount)
                    break;
            }
        }

        if (edgeBitsRead < totalBits)
            throw new ArgumentException("La chaîne d'entrée est trop courte pour représenter toutes les arêtes.");


        return graph.Build();
    }
}