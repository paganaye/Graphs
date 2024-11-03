namespace Graphs;

using System;
using System.Collections.Generic;

public class EdgeList
{
    // Serialize the graph into an edge list format: "(1,2), (1,3), (2,3), ..."
    public static string Serialize(Graph graph)
    {
        var edges = new List<string>();
        foreach (var n2 in graph.ForEachNode())
        {
            foreach (var n1 in graph.ForEachNeighbor(n2))
            {
                // Avoid duplicate edges by only adding if n1 < n2
                if (n1 < n2)
                {
                    edges.Add($"({n1 + 1},{n2 + 1})"); // Convert to one-based indexing
                }
                else break; // Stop if neighbors are not ordered, to avoid duplicate entries
            }
        }

        return string.Join(",", edges); // Join all edges into a single string
    }

    // Deserialize an edge list format string into a graph using GraphBuilder
    public static Graph Deserialize(int size, string edgeList)
    {
        // Regular expression to match edges in the form of "(1,2)"
        var edgePattern = new System.Text.RegularExpressions.Regex(@"\(\d+,\d+\)");
        var matches = edgePattern.Matches(edgeList);

        var builder = new GraphBuilder(size); // Initialize GraphBuilder with max node count

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            // Extract the edge string without parentheses
            var nodes = match.Value.Trim('(', ')').Split(',');

            if (nodes.Length == 2 &&
                int.TryParse(nodes[0], out int node1) &&
                int.TryParse(nodes[1], out int node2))
            {
                if (node1 <= size && node2 <= size)
                {
                    builder.SetEdge(node1 - 1, node2 - 1, true); // Convert to zero-based indexing
                }
            }
            else
            {
                throw new FormatException($"Invalid edge format: {match.Value}");
            }
        }

        return builder.Build(); // Return the completed graph
    }
}