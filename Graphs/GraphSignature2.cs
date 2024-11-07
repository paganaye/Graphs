namespace Graphs;

using System;
using System.Collections.Generic;
using System.Linq;

public class GraphSignature2
{
    private readonly Graph _graph;
    private readonly bool _inverted;
    private readonly Sig2Comparer _sigComparer = new();

    private List<Sig2> _unsortedNodeSignatures = [];
    private List<Sig2> _signature = [];

    public List<Sig2> UnsortedNodeSignatures => _unsortedNodeSignatures;
    public List<Sig2> Signature => _signature;
    
    public GraphSignature2(Graph graph)
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


    private List<Sig2> GetSortedNodeSignatures()
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
                expanded = ExpandSignatureTree(sig);
            } while (expanded);
        }

        _signature = sortedSignatures;

        // var filteredSignatures = new List<Sig2>();
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
        // void MarkNodesAsVisited(Sig2 sig)
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
            .Select(nodeIndex => Sig2.NewCollapsed(nodeIndex, _graph.GetNeighborCount(nodeIndex)))
            .ToList();
    }

    private List<List<Sig2>> GroupSignatures()
    {
        return _unsortedNodeSignatures
            .GroupBy(sig => sig, (key, group) => group.ToList(), _sigComparer)
            .ToList();
    }

    private bool ExpandAmbiguousGroups(List<List<Sig2>> groupedSignatures)
    {
        var expanded = false;

        foreach (var group in groupedSignatures.Where(it => it.Count > 1))
        {
            for (int i = 0; i < group.Count; i++)
            {
                var sig = group[i];
                var hasExpanded = ExpandSignatureTree(sig);
                if (hasExpanded)
                {
                    _unsortedNodeSignatures[sig.Node] = sig;
                    expanded = true;
                }
            }
        }

        return expanded;
    }

    private bool ExpandSignatureTree(Sig2 sig)
    {
        bool expanded = false;
        var visited = new int[_graph.NodeCount];

        bool Dfs(int level, ref Sig2 currentSig)
        {
            if (currentSig.Loop != 0) return false;
            else if (currentSig.Children == null)
            {
                var children = new List<Sig2>();
                foreach (var neighbor in _graph.ForEachNeighbor(currentSig.Node))
                {
                    if (visited[neighbor] > 0)
                        children.Add(Sig2.NewLoop(neighbor, level - visited[neighbor]));
                    else
                        children.Add(Sig2.NewCollapsed(neighbor, _graph.GetNeighborCount(neighbor)));
                }

                children.Sort(_sigComparer);

                currentSig.Children = children.ToArray();
                expanded = true;
                return true;
            }
            else
            {
                visited[currentSig.Node] = level;
                bool modified = false;
                for (int i = 0; i < currentSig.Children.Length; i++)
                {
                    modified |= Dfs(level + 1, ref currentSig.Children[i]);
                }

                if (modified) Array.Sort(currentSig.Children, _sigComparer);
                visited[currentSig.Node] = 0;
                return modified;
            }
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
    public static string CalculateSignature2(this Graph graph)
    {
        return new GraphSignature2(graph).ToString();
    }


}