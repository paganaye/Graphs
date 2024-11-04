using System;
using System.Reflection.Emit;

namespace Graphs;

using System.Collections.Generic;
using System.Linq;

public abstract class Sig(int node)
{
    public int Node = node; // this is used only temporarily to build the Signatures then it is all cleared at the end.
    public abstract int Order { get; }
    public abstract string DebugSig();
}

public class CollapsedSig(int node, int neighborCount) : Sig(node)
{
    public readonly int NeighborCount = neighborCount;

    public override string ToString()
    {
        return NeighborCount.ToString();
    }

    public override int Order => 1;

    public override string DebugSig()
    {
        return $"{NodeName.Get(node)}Neighbours({NeighborCount})";
    }
}

public class ExpandedSig(int node, Sig[] children) : Sig(node)
{
    public readonly Sig[] Children = children;

    public override string ToString()
    {
        // ReSharper disable once CoVariantArrayConversion
        return $"[{string.Join(",", (object[])Children)}]";
    }

    public override string DebugSig()
    {
        return
            $"{NodeName.Get(node)}Neighbours({Children.Length})[{String.Join(",", children.Select(c => c.DebugSig()))}]";
    }

    public override int Order => 2;
}

public class LoopSig : Sig
{
    public LoopSig(int node, int loop) : base(node)
    {
        if (loop >= 0) throw new InvalidProgramException("Loops should be negative");
        this.Loop = loop;
    }

    public int Loop { get; }

    public override string ToString()
    {
        return (Loop).ToString();
    }

    public override int Order => 3;

    public override string DebugSig()
    {
        return $"{NodeName.Get(Node)}:Loop({Loop})";
    }
}

public class SigComparer : IComparer<Sig>, IEqualityComparer<Sig>
{
    public int Compare(Sig x, Sig y)
    {
        if (x == null || y == null)
            throw new ArgumentNullException("Cannot compare null values.");

        // Handle LinkSig first as it has priority in the order
        if (x is LoopSig lx && y is LoopSig ly)
        {
            return lx.Loop.CompareTo(ly.Loop);
        }
        else if (x is LoopSig)
        {
            return -1; // LinkSig comes before any other Sig type
        }
        else if (y is LoopSig)
        {
            return 1; // Any other Sig type comes after LinkSig
        }

        // CollapsedSig vs CollapsedSig comparison
        if (x is CollapsedSig cx && y is CollapsedSig cy)
        {
            return cy.NeighborCount.CompareTo(cx.NeighborCount);
        }

        // ExpandedSig vs ExpandedSig comparison
        if (x is ExpandedSig ex && y is ExpandedSig ey)
        {
            int lengthComparison = ex.Children.Length.CompareTo(ey.Children.Length);
            if (lengthComparison != 0) return lengthComparison;

            // Recursively compare each child element if lengths are equal
            for (int i = 0; i < ex.Children.Length; i++)
            {
                int childComparison = Compare(ex.Children[i], ey.Children[i]);
                if (childComparison != 0) return childComparison;
            }

            return 0;
        }

        var orderResult = x.Order.CompareTo(y.Order);
        if (orderResult != 0) return orderResult;
        else throw new InvalidOperationException("Internal Error in SigComparer");
    }

    public bool Equals(Sig? x, Sig? y)
    {
        if (x == null || y == null)
            return false;

        if (ReferenceEquals(x, y))
            return true;

        if (x is LoopSig lx && y is LoopSig ly)
            return lx.Loop == ly.Loop;

        if (x is CollapsedSig cx && y is CollapsedSig cy)
            return cx.NeighborCount == cy.NeighborCount;

        if (x is ExpandedSig ex && y is ExpandedSig ey)
        {
            if (ex.Children.Length != ey.Children.Length)
                return false;

            for (int i = 0; i < ex.Children.Length; i++)
            {
                if (!Equals(ex.Children[i], ey.Children[i]))
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

        return obj switch
        {
            LoopSig lx => lx.Loop.GetHashCode(),
            CollapsedSig cx => cx.NeighborCount.GetHashCode(),
            ExpandedSig ex => ex.Children.Aggregate(0, (hash, child) => hash ^ GetHashCode(child)),
            _ => throw new InvalidOperationException("Unsupported Sig type")
        };
    }
}