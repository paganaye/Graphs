using System;
using System.Collections.Generic;
using System.Linq;

namespace Graphs;

using System.Collections.Generic;

public class Graph
{
    private readonly bool[,] _edges;
    private readonly int[] _neighborsCount;
    private readonly int[][] _neighbors;
    public int ActiveEdges { get; private set; }


    public Graph(GraphBuilder builder)
    {
        NodeCount = builder.NodeCount;
        MaxEdgeCount = (NodeCount * (NodeCount - 1)) / 2;
        _edges = new bool[NodeCount, NodeCount];
        InitEdges(builder);
        _neighbors = new int[NodeCount][];
        _neighborsCount = new int[NodeCount];
        PrecomputeNeighbors();
    }

    private void InitEdges(GraphBuilder builder)
    {
        this.ActiveEdges = 0;
        for (var j = 1; j < NodeCount; j++)
        {
            for (var i = 0; i < j; i++)
            {
                if (builder.HasEdge(i, j))
                {
                    _edges[i, j] = true;
                    _edges[j, i] = true;
                    this.ActiveEdges += 1;
                }
            }
        }
    }

    public int NodeCount { get; }

    public int MaxEdgeCount { get; }

    public bool HasEdge(int node1Index, int node2Index)
    {
        if (node1Index == node2Index) return false;
        if (node1Index < 0 || node1Index >= NodeCount || node2Index < 0 || node2Index >= NodeCount)
        {
            throw new ArgumentOutOfRangeException("Node indices must be within the valid range.");
        }

        return _edges[node1Index, node2Index];
    }

    public int GetNeighborCount(int nodeIndex)
    {
        return _neighborsCount[nodeIndex];
    }

    public IEnumerable<int> ForEachNeighbor(int nodeIndex)
    {
        return _neighbors[nodeIndex];
    }


    private void PrecomputeNeighbors()
    {
        for (int i = 0; i < NodeCount; i++)
        {
            List<int> nodeNeighbors = new List<int>();
            for (int j = 0; j < NodeCount; j++)
            {
                if (i != j && _edges[i, j])
                {
                    nodeNeighbors.Add(j);
                }
            }

            _neighbors[i] = nodeNeighbors.ToArray();
            _neighborsCount[i] = _neighbors[i].Length;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is Graph otherGraph)
            return Equals(otherGraph);

        return false;
    }

    public bool Equals(Graph? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (NodeCount != other.NodeCount || MaxEdgeCount != other.MaxEdgeCount)
            return false;

        for (int j = 1; j < NodeCount; j++)
        {
            for (int i = 0; i < j; i++)
            {
                if (_edges[i, j] != other._edges[i, j])
                    return false;
            }
        }

        return true;
    }
}

public static partial class GraphExtensions
{
    public static IEnumerable<int> ForEachNode(this Graph graph)
    {
        for (int i = 0; i < graph.NodeCount; i++)
        {
            yield return i;
        }
    }
}