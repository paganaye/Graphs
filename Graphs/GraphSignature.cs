using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Graphs;
using Tmds.DBus.Protocol;

public class GraphSignature
{
    private readonly Graph _graph;
    private readonly SigComparer _sigComparer = new SigComparer();

    public List<Sig> UnsortedNodeSignatures { get; private set; }
    public List<Sig> SortedNodeSignatures { get; private set; }

    public GraphSignature(Graph graph)
    {
        _graph = graph;

        UnsortedNodeSignatures = InitializeBaseSignatures(graph);
        int pass = 1;
        do
        {
            Console.WriteLine($"Pass: {pass}");
            var groupedSignatures = SortAndGroupSignatures();
            Console.WriteLine(
                $"Found {groupedSignatures.Count} groups of {String.Join(" ",
                    groupedSignatures.Select(g => g.Count))}");
            if (!ExpandAmbiguousGroups(groupedSignatures))
            {
                Console.WriteLine("No expansion are possible, we have a symetry in the graph.");
                break;
            }
        } while (!AreSignaturesUnique());

        UpdateSortedNodeSignatures();
        Console.WriteLine("Final signatures:", this.ToString());
    }

    private void UpdateSortedNodeSignatures()
    {
        SortedNodeSignatures = UnsortedNodeSignatures.OrderBy(s => s, _sigComparer).ToList();
    }

    private List<Sig> InitializeBaseSignatures(Graph graph)
    {
        return graph.ForEachNode()
            .Select(nodeIndex => new CollapsedSig(nodeIndex, graph.GetNeighborCount(nodeIndex)) { Node = nodeIndex })
            .Cast<Sig>()
            .ToList();
    }

    private List<List<Sig>> SortAndGroupSignatures()
    {
        return UnsortedNodeSignatures
            .GroupBy(sig => sig, (key, group) => group.ToList(), _sigComparer)
            .Where(group => group.Count > 1)
            .ToList();
    }

    private bool ExpandAmbiguousGroups(List<List<Sig>> groupedSignatures)
    {
        var expanded = false;

        foreach (var group in groupedSignatures.Where(it => it.Count > 1))
        {
            Console.WriteLine("Expanding group of ", group[0].ToString());
            for (int i = 0; i < group.Count; i++)
            {
                var sig = group[i];
                Console.WriteLine("Sig", sig);
                var hasExpanded = ExpandSignatureTree(ref sig);
                if (hasExpanded)
                {
                    Console.WriteLine("=>", sig);
                    UnsortedNodeSignatures[sig.Node] = sig;
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

        void Dfs(int level, ref Sig currentSig)
        {
            if (currentSig is CollapsedSig collapsedSig)
            {
                var children = new List<Sig>();
                foreach (var neighbor in _graph.ForEachNeighbor(currentSig.Node))
                {
                    if (visited[neighbor] > 0)
                        children.Add(new LoopSig(neighbor, visited[neighbor]));
                    else
                        children.Add(new CollapsedSig(neighbor, _graph.GetNeighborCount(neighbor)));
                }

                children.Sort(_sigComparer);

                currentSig = new ExpandedSig(currentSig.Node, children.ToArray());
                expanded = true;
            }
            else if (currentSig is ExpandedSig expandedSig)
            {
                visited[currentSig.Node] = level;
                for (int i = 0; i < expandedSig.Children.Length; i++)
                {
                    Dfs(level + 1, ref expandedSig.Children[i]);
                }

                Array.Sort(expandedSig.Children, _sigComparer);

                visited[currentSig.Node] = 0;
            }
        }

        Dfs(1, ref sig);
        return expanded;
    }

    private bool AreSignaturesUnique()
    {
        UpdateSortedNodeSignatures();

        for (int i = 1; i < SortedNodeSignatures.Count; i++)
        {
            if (_sigComparer.Compare(SortedNodeSignatures[i], SortedNodeSignatures[i - 1]) == 0)
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        return "[" + string.Join(",", SortedNodeSignatures.Select(s => s.ToString())) + "]";
    }

    public string DebugSig()
    {
        return "[" + string.Join(",", SortedNodeSignatures.Select(s => s.DebugSig())) + "]";
    }
}

public static class GraphExtensions
{
    public static string Signature(this Graph graph)
    {
        return new GraphSignature(graph).ToString();
    }
}