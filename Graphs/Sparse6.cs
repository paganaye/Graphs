using System;
using System.Text;

namespace Graphs;

public static class Sparse6
{
    public static string Serialize(Graph graph)
    {
        StringBuilder sb = new StringBuilder();
        int nodeCount = graph.NodeCount;

        sb.Append("s");
        sb.Append(nodeCount);

        for (int i = 0; i < nodeCount; i++)
        {
            for (int j = i + 1; j < nodeCount; j++)
            {
                if (graph.HasEdge(i, j))
                {
                    sb.Append(NodeName.Get(i));
                    sb.Append(NodeName.Get(j));
                }
            }
        }

        return sb.ToString();
    }

    public static void Deserialize(Graph graph, string sparse6)
    {
        if (sparse6[0] != 's')
        {
            throw new ArgumentException("Invalid Sparse6 format.");
        }

        int nodeCount = int.Parse(sparse6[1].ToString());
         graph.SetNodeCount(nodeCount);

        // Start reading from the 2nd character (after "s" and node count)
        for (int i = 2; i < sparse6.Length; i += 2)
        {
            char node1Char = sparse6[i];
            char node2Char = sparse6[i + 1];

            int node1Index = NodeName.GetIndex(node1Char);
            int node2Index = NodeName.GetIndex(node2Char);

            graph.SetEdge(node1Index, node2Index, true);
        }
    }
}