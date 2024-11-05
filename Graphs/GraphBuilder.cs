using System;

namespace Graphs;

public class GraphBuilder
{
    private readonly int _nodeCount;
    private readonly bool[][] _edges;

    public GraphBuilder(int nodeCount)
    {
        if (nodeCount < 1 || nodeCount > NodeName.MaxNodeCount)
        {
            throw new ArgumentException($"Node count must be between 1 and {NodeName.MaxNodeCount}");
        }

        _nodeCount = nodeCount;

        // Initialize the adjacency matrix as a triangular matrix
        // Each _edges[i] will have (_nodeCount - i - 1) elements representing edges (i, j) where j > i
        _edges = new bool[_nodeCount][];
        for (int j = 0; j < _nodeCount; j++)
        {
            _edges[j] = new bool[j];
        }
    }

    public int NodeCount => _nodeCount;

    /// <summary>
    /// Sets the edge between node i and node j.
    /// </summary>
    /// <param name="i">First node index (0-based).</param>
    /// <param name="j">Second node index (0-based).</param>
    /// <param name="value">True to create an edge, false to remove it.</param>
    public void SetEdge(int i, int j, bool value)
    {
        if (i == j)
        {
            throw new ArgumentException("No loops allowed (i.e., edges from a node to itself).");
        }

        if (i > j)
        {
            (i, j) = (j, i);
        }

        _edges[j][i] = value;
    }

    /// <summary>
    /// Checks if there is an edge between node i and node j.
    /// </summary>
    /// <param name="i">First node index (0-based).</param>
    /// <param name="j">Second node index (0-based).</param>
    /// <returns>True if an edge exists, otherwise false.</returns>
    public bool HasEdge(int i, int j)
    {
        if (i == j)
        {
            return false; // No loops
        }

        // Ensure i < j for the triangular matrix
        if (i > j)
        {
            (i, j) = (j, i);
        }

        return _edges[j][i];
    }

    /// <summary>
    /// Builds the Graph from the current state of the GraphBuilder.
    /// </summary>
    /// <returns>A new Graph instance.</returns>
    public Graph Build()
    {
        return new Graph(this);
    }


    public void Clear()
    {
        for (int j = 0; j < _nodeCount; j++)
        {
            Array.Fill(_edges[j], false);
        }
    }
}