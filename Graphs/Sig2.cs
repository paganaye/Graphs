using System;
using System.Reflection.Emit;

namespace Graphs;

using System.Collections.Generic;
using System.Linq;

public class Sig2
{
    public int Node; // this is used only temporarily to build the Signatures then it is all cleared at the end.
    public readonly int NeighborCount;
    public Sig2[]? Children = null;
    public int Loop;

    private Sig2(int node, int neighborCount, int loop = 0)
    {
        this.Node = node;
        this.NeighborCount = neighborCount;
        this.Loop = loop;
    }

    public override string ToString()
    {
        if (Loop != 0)
        {
            return $"L{Loop}";
        }

        if (Children == null)
        {
            return $"C{NeighborCount}";
        }
        else
        {
            return $"C{NeighborCount}[{string.Join(",", Children as object[])}]";
        }
    }

    public static Sig2 NewCollapsed(int i, int j)
    {
        return new Sig2(i, j);
    }

    public static Sig2 NewLoop(int node, int loop)
    {
        return new Sig2(node, 0, loop);
    }

    public static Sig2 NewExpanded(int node, List<Sig2> children)
    {
        var childrenArray = children.ToArray();
        var result = new Sig2(node,childrenArray.Length);
        result.Children = childrenArray;
        return result;
    }
}

public class Sig2Comparer : IComparer<Sig2>, IEqualityComparer<Sig2>
{
    public int Compare(Sig2 x, Sig2 y)
    {
        if (x == null || y == null)
            throw new ArgumentNullException("Cannot compare null values.");

        // Handle LinkSig first as it has priority in the order
        int result;
        result = y.Loop.CompareTo(x.Loop);
        if (result != 0) return result;

        result = y.NeighborCount.CompareTo(x.NeighborCount);
        if (result != 0) return result;

        if (x.Children != null && y.Children != null)
        {
            for (int i = 0; i < x.Children.Length; i++)
            {
                int childComparison = Compare(x.Children[i], y.Children[i]);
                if (childComparison != 0) return childComparison;
            }
        }

        return 0;
    }

    public bool Equals(Sig2? x, Sig2? y)
    {
        if (x == null) return (y == null) ? true : false;
        if (y == null) return false;
        return Compare(x, y) == 0;
    }

    public int GetHashCode(Sig2 obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var hash = obj.Loop ^ obj.NeighborCount;
        if (obj.Children != null)
        {
            foreach (var objChild in obj.Children)
                hash ^= objChild.GetHashCode();
        }

        return hash;
    }
}