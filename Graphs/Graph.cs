using System;

namespace Graphs;

public class Graph
{
    private readonly int _nodeCount;
    private readonly int _edgeCount;
    private readonly bool[] _edges;

    public Graph(int nodeCount)
    {
        if (nodeCount < 1 || nodeCount > 52)
        {
            throw new ArgumentException("Node count must be between 1 and 52.");
        }

        _nodeCount = nodeCount;
        _edgeCount = nodeCount * (nodeCount - 1) / 2;
        _edges = new bool[_edgeCount];
    }

    public int NodeCount => _nodeCount;
    public int EdgeCount => _edgeCount;

    public void SetEdge(char node1, char node2, bool value)
    {
        int edgeIndex = EdgeName.GetEdgeIndex(node1, node2);
        this._edges[edgeIndex] = value;
    }

    public void SetEdge(int node1, int node2, bool value)
    {
        int edgeIndex = EdgeName.GetEdgeIndex(node1, node2);
        this._edges[edgeIndex] = value;
    }


    public bool HasEdge(char node1, char node2)
    {
        int edgeIndex = EdgeName.GetEdgeIndex(node1, node2);
        return _edges[edgeIndex];
    }

    public bool HasEdge(int node1Index, int node2Index)
    {
        int edgeIndex = EdgeName.GetEdgeIndex(node1Index, node2Index);
        return edgeIndex >= 0 && _edges[edgeIndex];
    }

    public void DisplayMatrix()
    {
        Console.Write("  ");
        for (int i = 0; i < _nodeCount; i++)
        {
            Console.Write(NodeName.Get(i) + " ");
        }

        Console.WriteLine();

        for (int i = 0; i < _nodeCount; i++)
        {
            Console.Write(NodeName.Get(i) + " ");
            for (int j = 0; j < _nodeCount; j++)
            {
                Console.Write((HasEdge(i, j) ? "1" : "0") + " ");
            }

            Console.WriteLine();
        }
    }
}