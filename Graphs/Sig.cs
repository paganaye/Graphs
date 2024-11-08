using System;
using System.Reflection.Emit;

namespace Graphs;

using System.Collections.Generic;
using System.Linq;

public enum SigType
{
    Loop = 3,
    Expanded = 2,
    Collapsed = 1
}

public class Sig(int node)
{
    public int Node = node; // this is used only temporarily to build the Signatures then it is all cleared at the end.

    public SigType SigType { get; private set; }

    public Sig[]? Neighbors { get; private set; }

    public int Loop { get; private set; }

    public int NeighborCount { get; private set; }

    public static Sig NewCollapsedSig(int node, int neighborCount)
    {
        return new Sig(node) { NeighborCount = neighborCount, SigType = SigType.Collapsed };
    }

    public static Sig NewExpandedSig(int node, Sig[] neighbors)
    {
        return new Sig(node) { Neighbors = neighbors, NeighborCount = neighbors.Length, SigType = SigType.Expanded };
    }

    public static Sig NewLoopSig(int node, int loop)
    {
        return new Sig(node) { Loop = loop, SigType = SigType.Loop };
    }

    public override string ToString()
    {
        return SigType switch
        {
            SigType.Loop => (Loop).ToString(),
            SigType.Collapsed => NeighborCount.ToString(),
            // ReSharper disable once CoVariantArrayConversion
            SigType.Expanded => $"[{string.Join(",", (object[])Neighbors!)}]",
        };
    }

    public void Expand(Sig[] neighbors)
    {
        if (neighbors.Length != NeighborCount) throw new InvalidOperationException();
        this.SigType = SigType.Expanded;
        this.Neighbors = neighbors;
    }
}

public class SigComparer : IComparer<Sig>, IEqualityComparer<Sig>
{
    public int Compare(Sig x, Sig y)
    {
        if (x == null || y == null)
            throw new ArgumentNullException("Cannot compare null values.");

        // Handle LinkSig first as it has priority in the order
        if (x.SigType == SigType.Loop && y.SigType == SigType.Loop)
        {
            return x.Loop.CompareTo(y.Loop);
        }
        else if (x.SigType == SigType.Loop)
        {
            return -1; // LinkSig comes before any other Sig type
        }
        else if (y.SigType == SigType.Loop)
        {
            return 1; // Any other Sig type comes after LinkSig
        }

        // CollapsedSig vs CollapsedSig comparison
        if (x.SigType == SigType.Collapsed && y.SigType == SigType.Collapsed)
        {
            return y.NeighborCount.CompareTo(x.NeighborCount);
        }

        // ExpandedSig vs ExpandedSig comparison
        if (x.SigType == SigType.Expanded && y.SigType == SigType.Expanded)
        {
            int lengthComparison = x.Neighbors!.Length.CompareTo(y.Neighbors!.Length);
            if (lengthComparison != 0) return lengthComparison;

            // Recursively compare each child element if lengths are equal
            for (int i = 0; i < x.Neighbors.Length; i++)
            {
                int childComparison = Compare(x.Neighbors[i], y.Neighbors[i]);
                if (childComparison != 0) return childComparison;
            }

            return 0;
        }

        var orderResult = x.SigType.CompareTo(y.SigType);
        if (orderResult != 0) return orderResult;
        else throw new InvalidOperationException("Internal Error in SigComparer");
    }

    public bool Equals(Sig? x, Sig? y)
    {
        if (x == null || y == null)
            return false;

        if (ReferenceEquals(x, y))
            return true;

        if (x.SigType == SigType.Loop && y.SigType == SigType.Loop)
            return x.Loop == y.Loop;

        if (x.SigType == SigType.Collapsed && y.SigType == SigType.Collapsed)
            return x.NeighborCount == y.NeighborCount;

        if (x.SigType == SigType.Expanded && y.SigType == SigType.Expanded)
        {
            if (x.Neighbors!.Length != y.Neighbors!.Length)
                return false;

            for (int i = 0; i < x.Neighbors.Length; i++)
            {
                if (!Equals(x.Neighbors[i], y.Neighbors[i]))
                    return false;
            }

            return true;
        }

        return false;
    }

    public int GetHashCode(Sig obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (obj.SigType == SigType.Loop) return obj.Loop.GetHashCode();
        else if (obj.SigType == SigType.Collapsed) return obj.NeighborCount.GetHashCode();
        else if (obj.SigType == SigType.Expanded)
            return obj.Neighbors!.Aggregate(0, (hash, child) => hash ^ GetHashCode(child));
        else throw new InvalidOperationException("Unsupported Sig type");
    }
}