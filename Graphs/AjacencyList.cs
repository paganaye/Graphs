using System.Linq;

namespace Graphs;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class AdjacencyList
{
    // Serialize the graph into an adjacency list format: "1: 2 3\n2: 1 3\n3: 1 2\n..."
    public static string Serialize(Graph graph)
    {
        var result = new List<string>();
        List<int> list = [];

        // Populate the adjacency list dictionary
        foreach (var n2 in graph.ForEachNode())
        {
            result.Add(
                $"{n2+1}: {string.Join(" ", graph.ForEachNeighbor(n2).Select(n => (n + 1).ToString()))}");
        }
        return string.Join("\n", result); // Join all entries into a single string
    }

    // Deserialize an adjacency list format string into a graph using GraphBuilder
    public static Graph? Deserialize(string adjacencyList)
    {
        var lines = adjacencyList.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int nodeMax = 0;
        var edges = new List<(int from, int to)>(); // Temporarily store edges

        foreach (var line in lines)
        {
            var parts = line.Split(new[] { ':' }, 2);
            if (parts.Length != 2)
            {
                throw new FormatException($"Format invalide de la liste d'adjacence : {line}");
            }

            if (!int.TryParse(parts[0].Trim(), out int node))
            {
                throw new FormatException($"Format de nœud invalide dans la ligne : {line}");
            }

            if (node > nodeMax) nodeMax = node;

            var neighbors = parts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var neighbor in neighbors)
            {
                if (!int.TryParse(neighbor, out int adjacentNode))
                {
                    throw new FormatException($"Format de nœud voisin invalide dans la ligne : {line}");
                }

                if (adjacentNode > nodeMax) nodeMax = adjacentNode;
                edges.Add((node, adjacentNode));
            }
        }

        var builder = new GraphBuilder(nodeMax);

        foreach (var (from, to) in edges)
        {
            builder.SetEdge(from - 1, to - 1, true);
        }

        return builder.Build();
    }
}