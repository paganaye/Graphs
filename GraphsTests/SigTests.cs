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
        var signatures = GetSignature(Sig.NewLoopSig(0, -1), Sig.NewLoopSig(1, -2));
        Assert.That(signatures, Is.EqualTo("[-2,-1]"));
    }

    [Test]
    public void LinkSig_comes_before_collapsed_sig()
    {
        var signatures = GetSignature(Sig.NewLoopSig(0, -1), Sig.NewCollapsedSig(1, 2));
        Assert.That(signatures, Is.EqualTo("[-1,2]"));
    }

    [Test]
    public void LinkSig_comes_before_expanded_sig()
    {
        var signatures = GetSignature(Sig.NewLoopSig(0, -1), Sig.NewExpandedSig(1, []));
        Assert.That(signatures, Is.EqualTo("[-1,[]]"));
    }

    [Test]
    public void CollapsedSig_with_more_eges_comes_first()
    {
        var signatures = GetSignature(Sig.NewCollapsedSig(0, 2), Sig.NewCollapsedSig(1, 1));
        Assert.That(signatures, Is.EqualTo("[2,1]"));
    }

    [Test]
    public void CollapsedSig_cannot_be_compared_with_expanded_sig()
    {
        Assert.That(() => GetSignature(Sig.NewCollapsedSig(0, 2), Sig.NewExpandedSig(1, new Sig[] { })),
            Is.EqualTo("[2,[]]"));
    }

    [Test]
    public void ExpandedSig_cannot_be_compared_with_expanded_sig_of_another_size()
    {
        Assert.That(() => GetSignature(Sig.NewExpandedSig(0, [Sig.NewCollapsedSig(1, 1)]), Sig.NewExpandedSig(2, new Sig[] { })),
            Is.EqualTo("[[],[1]]"));
    }

    [Test]
    public void ExpandedSig_comparison()
    {
        // [[2,2],[2,1]] => [[2,2],[2,1]]
        var signatures = GetSignature(Sig.NewExpandedSig(0, [Sig.NewCollapsedSig(1, 2), Sig.NewCollapsedSig(2, 2)]),
            Sig.NewExpandedSig(5, [Sig.NewCollapsedSig(6, 2), Sig.NewCollapsedSig(7, 1)]));
        Assert.That(signatures, Is.EqualTo("[[2,2],[2,1]]"));

        // [[2,1],[2,2]] => [[2,2],[2,1]]
        signatures = GetSignature(Sig.NewExpandedSig(0, [Sig.NewCollapsedSig(1, 2), Sig.NewCollapsedSig(2, 1)]),
            Sig.NewExpandedSig(0, [Sig.NewCollapsedSig(1, 2), Sig.NewCollapsedSig(2, 2)]));
        Assert.That(signatures, Is.EqualTo("[[2,2],[2,1]]"));
    }

    [Test]
    public void ExpandedSig_with_same_length_but_different_elements()
    {
        var signatures = GetSignature(
            Sig.NewExpandedSig(0, new Sig[] { Sig.NewCollapsedSig(1, 2), Sig.NewLoopSig(2, -1) }),
            Sig.NewExpandedSig(0, new Sig[] { Sig.NewCollapsedSig(1, 1), Sig.NewLoopSig(2, -1) })
        );
        Assert.That(signatures, Is.EqualTo("[[2,-1],[1,-1]]"));
    }

    [Test]
    public void NestedExpandedSig()
    {
        var signatures = GetSignature(
            Sig.NewExpandedSig(0, new Sig[] { Sig.NewExpandedSig(1, new Sig[] { Sig.NewCollapsedSig(2, 2) }) }),
            Sig.NewExpandedSig(0, new Sig[] { Sig.NewExpandedSig(1, new Sig[] { Sig.NewCollapsedSig(2, 1) }) })
        );
        Assert.That(signatures, Is.EqualTo("[[[2]],[[1]]]"));
    }

    [Test]
    public void MultipleCollapsedSig_in_order()
    {
        var signatures = GetSignature(Sig.NewCollapsedSig(1, 3), Sig.NewCollapsedSig(2, 1), Sig.NewCollapsedSig(3, 2));
        Assert.That(signatures, Is.EqualTo("[3,2,1]"));
    }

    [Test]
    public void ExpandedSig_with_identical_elements()
    {
        var signatures = GetSignature(
            Sig.NewExpandedSig(0, new Sig[] { Sig.NewCollapsedSig(1, 2), Sig.NewCollapsedSig(2, 1) }),
            Sig.NewExpandedSig(0, new Sig[] { Sig.NewCollapsedSig(1, 2), Sig.NewCollapsedSig(2, 1) })
        );
        Assert.That(signatures, Is.EqualTo("[[2,1],[2,1]]"));
    }

    [Test]
    public void ExpandedSig_with_different_LinkSig_levels()
    {
        var signatures = GetSignature(
            Sig.NewExpandedSig(0, new Sig[] { Sig.NewLoopSig(1, -2), Sig.NewLoopSig(2, -1) }),
            Sig.NewExpandedSig(0, new Sig[] { Sig.NewLoopSig(1, -1), Sig.NewLoopSig(2, -2) })
        );
        Assert.That(signatures, Is.EqualTo("[[-2,-1],[-1,-2]]"));
    }

    [Test]
    public void EmptyExpandedSig_and_CollapsedSig()
    {
        Assert.That(() => GetSignature(Sig.NewExpandedSig(0, new Sig[] { }), Sig.NewCollapsedSig(1, 0)),
            Is.EqualTo("[0,[]]"));
    }
}