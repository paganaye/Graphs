using System;
using System.Collections.Generic;

namespace Graphs;

internal static class SimpleIsomorphic
{
    public static bool Compare(Graph graph1, Graph graph2)
    {
        return AreIsomorphic(graph1, graph2);
    }

    public static bool AreIsomorphic(Graph graph1, Graph graph2)
    {
        if (graph1.NodeCount != graph2.NodeCount) return false;
        if (graph1.NodeCount > 10) throw new Exception("SimpleIsomorphic only supports up to 10 nodes");

        var permutations = GeneratePermutations(graph1.NodeCount);
        foreach (var permutation in permutations)
        {
            if (ArePermutedGraphsIdentical(graph1, graph2, permutation))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<int[]> GeneratePermutations(int n)
    {
        var result = new List<int[]>();
        Permute(new List<int>(), new HashSet<int>(), n, result);
        return result;
    }

    private static void Permute(List<int> current, HashSet<int> used, int n, List<int[]> result)
    {
        if (current.Count == n)
        {
            result.Add(current.ToArray());
            return;
        }

        for (int i = 0; i < n; i++)
        {
            if (used.Contains(i)) continue;

            current.Add(i);
            used.Add(i);
            Permute(current, used, n, result);
            current.RemoveAt(current.Count - 1);
            used.Remove(i);
        }
    }

    private static bool ArePermutedGraphsIdentical(Graph graph1, Graph graph2, int[] permutation)
    {
        for (int i = 0; i < graph1.NodeCount; i++)
        {
            for (int j = 0; j < graph1.NodeCount; j++)
            {
                int permutedI = permutation[i];
                int permutedJ = permutation[j];

                if (graph1.HasEdge(i, j) != graph2.HasEdge(permutedI, permutedJ))
                {
                    return false;
                }
            }
        }

        return true;
    }
}