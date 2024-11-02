namespace Graphs;

using System;
using System.Text;

public static class Graph6
{
    public static string Serialize(Graph graph)
    {
        int nodeCount = graph.NodeCount;
        StringBuilder sb = new StringBuilder();

        sb.Append("g");
        sb.Append(nodeCount);

        int edgeCount =
            nodeCount * (nodeCount - 1) /
            2; // Le nombre total d'arêtes possibles dans un graphe non orienté sans boucles
        char[] edges = new char[(edgeCount + 5) / 6]; // 6 bits par caractère

        int edgeIndex = 0;
        for (int j = 0; j < nodeCount; j++)
        {
            for (int i = 0; i < j; i++)
            {
                if (graph.HasEdge(i, j))
                {
                    edges[edgeIndex / 6] |= (char)(1 << (edgeIndex % 6));
                }

                edgeIndex++;
            }
        }

        foreach (var edge in edges)
        {
            sb.Append((char)(edge + 63)); // Convertir en caractères ASCII en utilisant l'offset de 63
        }

        return sb.ToString();
    }

    public static Graph Deserialize(string graph6)
    {
        if (graph6[0] != 'g')
        {
            throw new ArgumentException("Invalid Graph6 format.");
        }

        // Read the number of nodes (could be multiple digits)
        int currentIndex = 1;
        StringBuilder nodeCountBuilder = new StringBuilder();

        while (char.IsDigit(graph6[currentIndex]))
        {
            nodeCountBuilder.Append(graph6[currentIndex]);
            currentIndex++;
        }

        int nodeCount = int.Parse(nodeCountBuilder.ToString());
        Graph graph = new Graph(nodeCount);

        // Read edges starting from where the edge characters begin
        char[] edgeChars = graph6.Substring(currentIndex).ToCharArray();
        int edgeIndex = 0;

        foreach (var edgeChar in edgeChars)
        {
            int edgeBits = edgeChar - 63; // Convert from ASCII

            for (int bit = 0; bit < 6; bit++)
            {
                if (edgeIndex >= (nodeCount * (nodeCount - 1) / 2)) break; // Ensure we don't exceed the number of edges

                if ((edgeBits & (1 << bit)) != 0)
                {
                    // Calculate node1 and node2 based on edge index
                    int node1 = 0;
                    int node2 = 0;
                    int tempIndex = edgeIndex;

                    // Modified logic to correctly find node1 and node2 based on the updated serialize method
                    for (node2 = 1; node2 < nodeCount; node2++)
                    {
                        if (tempIndex < node2)
                        {
                            node1 = tempIndex;
                            break;
                        }

                        tempIndex -= node2;
                    }

                    graph.SetEdge(node1, node2, true);
                }

                edgeIndex++;
            }
        }

        return graph;
    }
}