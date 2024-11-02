namespace Graphs;

using System;

public static class EdgeName
{
    private static readonly string[] EdgeNames;
    private static readonly int[,] EdgeIndices;

    static EdgeName()
    {
        int nodeCount = NodeName.MaxNodeCount;
        int edgeCount = nodeCount * (nodeCount - 1) / 2;
        EdgeNames = new string[edgeCount];
        EdgeIndices = new int[nodeCount, nodeCount];
        int index = 0;
        for (int j = 0; j < nodeCount; j++)
        {
            for (int i = 0; i < j; i++)
            {
                EdgeNames[index] = $"{NodeName.Get(i)}{NodeName.Get(j)}";
                if (i < j)
                {
                    EdgeIndices[i, j] = index;
                    EdgeIndices[j, i] = index;
                    index++;
                }
            }

            EdgeIndices[j, j] = -1;
        }
    }

    public static string Get(int edgeIndex)
    {
        if (edgeIndex < 0) throw new ArgumentException($"Edge does not exist.");
        return EdgeNames[edgeIndex];
    }

    public static string Get(int node1Index, int node2Index)
    {
        var edgeIndex = EdgeIndices[node1Index, node2Index];
        return Get(edgeIndex);
    }

    public static string Get(char node1Name, char node2Name)
    {
        int node1Index = NodeName.GetIndex(node1Name);
        int node2Index = NodeName.GetIndex(node2Name);
        return Get(node1Index, node2Index);
    }

    public static int GetEdgeIndex(int node1Index, int node2Index)
    {
        return EdgeIndices[node1Index, node2Index];
    }
}