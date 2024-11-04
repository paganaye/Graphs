namespace GraphsTests;

using Graphs;

[TestFixture]
public class SigTests
{
    private string GetSignature(params Sig[] signatures)
    {
        var signatureList = signatures.ToList();
        signatureList.Sort(new SigComparer());
        return "[" + string.Join(",", signatureList) + "]";
    }

    [Test]
    public void LinkSig_with_longer_loop_comes_first()
    {
        var signatures = GetSignature(new LoopSig(0, -1), new LoopSig(1, -2));
        Assert.That(signatures, Is.EqualTo("[-2,-1]"));
    }

    [Test]
    public void LinkSig_comes_before_collapsed_sig()
    {
        var signatures = GetSignature(new LoopSig(0, -1), new CollapsedSig(1, 2));
        Assert.That(signatures, Is.EqualTo("[-1,2]"));
    }

    [Test]
    public void LinkSig_comes_before_expanded_sig()
    {
        var signatures = GetSignature(new LoopSig(0, -1), new ExpandedSig(1, []));
        Assert.That(signatures, Is.EqualTo("[-1,[]]"));
    }

    [Test]
    public void CollapsedSig_with_more_eges_comes_first()
    {
        var signatures = GetSignature(new CollapsedSig(0, 2), new CollapsedSig(1, 1));
        Assert.That(signatures, Is.EqualTo("[2,1]"));
    }

    [Test]
    public void CollapsedSig_cannot_be_compared_with_expanded_sig()
    {
        Assert.That(() => GetSignature(new CollapsedSig(0, 2), new ExpandedSig(1, new Sig[] { })),
            Is.EqualTo("[2,[]]"));
    }

    [Test]
    public void ExpandedSig_cannot_be_compared_with_expanded_sig_of_another_size()
    {
        Assert.That(() => GetSignature(new ExpandedSig(0, [new CollapsedSig(1, 1)]), new ExpandedSig(2, new Sig[] { })),
            Is.EqualTo("[[],[1]]"));
    }

    [Test]
    public void ExpandedSig_comparison()
    {
        // [[2,2],[2,1]] => [[2,2],[2,1]]
        var signatures = GetSignature(new ExpandedSig(0, [new CollapsedSig(1, 2), new CollapsedSig(2, 2)]),
            new ExpandedSig(5, [new CollapsedSig(6, 2), new CollapsedSig(7, 1)]));
        Assert.That(signatures, Is.EqualTo("[[2,2],[2,1]]"));

        // [[2,1],[2,2]] => [[2,2],[2,1]]
        signatures = GetSignature(new ExpandedSig(0, [new CollapsedSig(1, 2), new CollapsedSig(2, 1)]),
            new ExpandedSig(0, [new CollapsedSig(1, 2), new CollapsedSig(2, 2)]));
        Assert.That(signatures, Is.EqualTo("[[2,2],[2,1]]"));
    }

    [Test]
    public void ExpandedSig_with_same_length_but_different_elements()
    {
        var signatures = GetSignature(
            new ExpandedSig(0, new Sig[] { new CollapsedSig(1, 2), new LoopSig(2, -1) }),
            new ExpandedSig(0, new Sig[] { new CollapsedSig(1, 1), new LoopSig(2, -1) })
        );
        Assert.That(signatures, Is.EqualTo("[[2,-1],[1,-1]]"));
    }

    [Test]
    public void NestedExpandedSig()
    {
        var signatures = GetSignature(
            new ExpandedSig(0, new Sig[] { new ExpandedSig(1, new Sig[] { new CollapsedSig(2, 2) }) }),
            new ExpandedSig(0, new Sig[] { new ExpandedSig(1, new Sig[] { new CollapsedSig(2, 1) }) })
        );
        Assert.That(signatures, Is.EqualTo("[[[2]],[[1]]]"));
    }

    [Test]
    public void MultipleCollapsedSig_in_order()
    {
        var signatures = GetSignature(new CollapsedSig(1, 3), new CollapsedSig(2, 1), new CollapsedSig(3, 2));
        Assert.That(signatures, Is.EqualTo("[3,2,1]"));
    }

    [Test]
    public void ExpandedSig_with_identical_elements()
    {
        var signatures = GetSignature(
            new ExpandedSig(0, new Sig[] { new CollapsedSig(1, 2), new CollapsedSig(2, 1) }),
            new ExpandedSig(0, new Sig[] { new CollapsedSig(1, 2), new CollapsedSig(2, 1) })
        );
        Assert.That(signatures, Is.EqualTo("[[2,1],[2,1]]"));
    }

    [Test]
    public void ExpandedSig_with_different_LinkSig_levels()
    {
        var signatures = GetSignature(
            new ExpandedSig(0, new Sig[] { new LoopSig(1, -2), new LoopSig(2, -1) }),
            new ExpandedSig(0, new Sig[] { new LoopSig(1, -1), new LoopSig(2, -2) })
        );
        Assert.That(signatures, Is.EqualTo("[[-2,-1],[-1,-2]]"));
    }

    [Test]
    public void EmptyExpandedSig_and_CollapsedSig()
    {
        Assert.That(() => GetSignature(new ExpandedSig(0, new Sig[] { }), new CollapsedSig(1, 0)),
            Is.EqualTo("[0,[]]"));
    }
}