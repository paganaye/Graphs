namespace Graphs;

using System;

public static class NodeName
{
    public  const int MaxNodeCount = 52;
    private static readonly string[] NodeNames = new string[MaxNodeCount];
    private static readonly int[] NodeIndices = new int[128]; // A-Z a-z ascii

    static NodeName()
    {
        // Precalculations
        int index = 0;
        for (char c = 'A'; c <= 'Z'; c++)
        {
            NodeNames[index] = c.ToString();
            NodeIndices[c] = index++;
        }

        for (char c = 'a'; c <= 'z'; c++)
        {
            NodeNames[index] = c.ToString();
            NodeIndices[c] = index++;
        }
    }

    public static string Get(int nodeIndex)
    {
        return NodeNames[nodeIndex];
    }

    public static int GetIndex(char nodeName)
    {
        var index = NodeIndices[nodeName ];
        return index;
    }
}