namespace Graphs;

using System;
using System.Collections.Generic;
using System.Linq;

public class GraphSignature
{
    private readonly Graph _graph;
    private readonly bool _inverted;
    private readonly SigComparer _sigComparer = new();

    private List<Sig> _unsortedNodeSignatures = [];
    private List<Sig> _signature = [];

    public List<Sig> UnsortedNodeSignatures => _unsortedNodeSignatures;
    public List<Sig> Signature => _signature;

    public GraphSignature(Graph graph)
    {
        _inverted = (graph.GetActiveEdgeCount() > graph.MaxEdgeCount / 2);
        _graph = _inverted ? graph.Invert() : graph;

        InitializeBaseSignatures();
        do
        {
            var groupedSignatures = GroupSignatures();
            if (!ExpandAmbiguousGroups(groupedSignatures)) break;
        } while (!AreSignaturesUnique());

        GetFinalSignatures();
    }


    private List<Sig> GetSortedNodeSignatures()
    {
        return _unsortedNodeSignatures.OrderBy(s => s, _sigComparer).ToList();
    }

    private void GetFinalSignatures()
    {
        var sortedSignatures = GetSortedNodeSignatures();
        for (var i = 0; i < _graph.NodeCount; i++)
        {
            var sig = sortedSignatures[i];
            var expanded = false;
            do
            {
                expanded = ExpandSignatureTree(ref sig);
                if (expanded) sortedSignatures[i] = sig;
            } while (expanded);
        }

        _signature = sortedSignatures;

        // var filteredSignatures = new List<Sig>();
        // foreach (var signature in sortedSignatures)
        // {
        //     if (!visitedNodes[signature.Node])
        //     {
        //         filteredSignatures.Add(signature);
        //         MarkNodesAsVisited(signature);
        //     }
        // }
        //
        // _signature = filteredSignatures;
        //
        // void MarkNodesAsVisited(Sig sig)
        // {
        //     if (visitedNodes[sig.Node]) return;
        //     visitedNodes[sig.Node] = true;
        //     if (sig is ExpandedSig expandedSig)
        //     {
        //         foreach (var child in expandedSig.Children)
        //         {
        //             MarkNodesAsVisited(child);
        //         }
        //     }
        // }
    }


    private void InitializeBaseSignatures()
    {
        _unsortedNodeSignatures = _graph.ForEachNode()
            .Select(nodeIndex => Sig.NewCollapsedSig(nodeIndex, _graph.GetNeighborCount(nodeIndex)))
            .Cast<Sig>()
            .ToList();
    }

    private List<List<Sig>> GroupSignatures()
    {
        return _unsortedNodeSignatures
            .GroupBy(sig => sig, (key, group) => group.ToList(), _sigComparer)
            .ToList();
    }

    private bool ExpandAmbiguousGroups(List<List<Sig>> groupedSignatures)
    {
        var expanded = false;

        foreach (var group in groupedSignatures.Where(it => it.Count > 1))
        {
            for (int i = 0; i < group.Count; i++)
            {
                var sig = group[i];
                var hasExpanded = ExpandSignatureTree(ref sig);
                if (hasExpanded)
                {
                    _unsortedNodeSignatures[sig.Node] = sig;
                    expanded = true;
                }
            }
        }

        return expanded;
    }

    private bool ExpandSignatureTree(ref Sig sig)
    {
        bool expanded = false;
        var visited = new int[_graph.NodeCount];

        bool Dfs(int level, ref Sig currentSig)
        {
            if (currentSig.SigType == SigType.Collapsed)
            {
                var children = new List<Sig>();
                foreach (var neighbor in _graph.ForEachNeighbor(currentSig.Node))
                {
                    if (visited[neighbor] > 0)
                        children.Add(Sig.NewLoopSig(neighbor, visited[neighbor] - level));
                    else
                        children.Add(Sig.NewCollapsedSig(neighbor, _graph.GetNeighborCount(neighbor)));
                }

                children.Sort(_sigComparer);
                currentSig.Expand(children.ToArray());
                expanded = true;
                return true;
            }
            else if (currentSig.SigType == SigType.Expanded)
            {
                visited[currentSig.Node] = level;
                bool modified = false;
                for (int i = 0; i < currentSig.Neighbors.Length; i++)
                {
                    modified |= Dfs(level + 1, ref currentSig.Neighbors[i]);
                }

                if (modified) Array.Sort(currentSig.Neighbors, _sigComparer);
                visited[currentSig.Node] = 0;
                return modified;
            }
            else return false;
        }

        Dfs(1, ref sig);
        return expanded;
    }

    private bool AreSignaturesUnique()
    {
        var currentSignatures = GetSortedNodeSignatures();
        for (int i = 1; i < currentSignatures.Count; i++)
        {
            if (_sigComparer.Compare(currentSignatures[i], currentSignatures[i - 1]) == 0)
                return false;
        }

        return true;
    }

    public override string ToString()
    {
        return $"{(_inverted ? "!" : "")}[{string.Join(",", _signature.Select(s => s.ToString()))}]";
    }
}

public static partial class GraphExtensions
{
    public static string CalculateSignature(this Graph graph)
    {
        return new GraphSignature(graph).ToString();
    }

    public static Graph Invert(this Graph graph)
    {
        var count = graph.NodeCount;
        var newGraph = new GraphBuilder(count);
        for (int j = 1; j < count; j++)
        {
            for (int i = 0; i < j; i++)
            {
                if (!graph.HasEdge(i, j))
                {
                    newGraph.SetEdge(i, j, true);
                }
            }
        }

        return newGraph.Build();
    }

    public static int GetActiveEdgeCount(this Graph? graph)
    {
        if (graph == null) return 0;
        int count = 0;
        foreach (int node in graph.ForEachNode())
        {
            foreach (int neighbor in graph.ForEachNeighbor(node))
            {
                if (neighbor < node) count++;
                else break;
            }
        }

        return count;
    }
}