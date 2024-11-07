namespace GraphsTests;

using Graphs;

[TestFixture]
public class Sig2Tests
{
    private string GetSignature(params Sig2[] signatures)
    {
        var signatureList = signatures.ToList();
        signatureList.Sort(new Sig2Comparer());
        return string.Join(" ", signatureList);
    }

    [Test]
    public void LinkSig_with_longer_loop_comes_first()
    {
        var signatures = GetSignature(Sig2.NewLoop(0, 1), Sig2.NewLoop(1, 2));
        Assert.That(signatures, Is.EqualTo("L2 L1"));
    }

    [Test]
    public void LinkSig_comes_before_collapsed_sig()
    {
        var signatures = GetSignature(Sig2.NewLoop(0, 2), Sig2.NewCollapsed(1, 2));
        Assert.That(signatures, Is.EqualTo("L2 C2"));
    }

    [Test]
    public void LinkSig_comes_before_expanded_sig()
    {
        var signatures = GetSignature(Sig2.NewLoop(0, 1), Sig2.NewExpanded(1, []));
        Assert.That(signatures, Is.EqualTo("L1 C0[]"));
    }

    [Test]
    public void CollapsedSig_with_more_edges_comes_first()
    {
        var signatures = GetSignature(Sig2.NewCollapsed(0, 2), Sig2.NewCollapsed(1, 1));
        Assert.That(signatures, Is.EqualTo("C2 C1"));
    }

    [Test]
    public void CollapsedSig_with_more_edges_comes_before_expanded_with_less_edges()
    {
        Assert.That(() => GetSignature(Sig2.NewCollapsed(0, 2), Sig2.NewExpanded(1, [])),
            Is.EqualTo("C2 C0[]"));
    }


    [Test]
    public void ExpandedSig_comparison()
    {
        // C2[C2,C2] C2[C2,C1] => C2[C2,C2] C2[C2,C1]
        var signatures = GetSignature(
            Sig2.NewExpanded(0, [Sig2.NewCollapsed(1, 2), Sig2.NewCollapsed(2, 2)]),
            Sig2.NewExpanded(5, [Sig2.NewCollapsed(6, 2), Sig2.NewCollapsed(7, 1)]));
        Assert.That(signatures, Is.EqualTo("C2[C2,C2] C2[C2,C1]"));

        // C2[C2,C1] C2[C2,C2] => C2[C2,C2] C2[C2,C1]
        signatures = GetSignature(
            Sig2.NewExpanded(0, [Sig2.NewCollapsed(1, 2), Sig2.NewCollapsed(2, 1)]),
            Sig2.NewExpanded(0, [Sig2.NewCollapsed(1, 2), Sig2.NewCollapsed(2, 2)]));
        Assert.That(signatures, Is.EqualTo("C2[C2,C2] C2[C2,C1]"));
    }
//
//     [Test]
//     public void ExpandedSig_with_same_length_but_different_elements()
//     {
//         var signatures = GetSignature(
//             Sig2.NewExpanded(0, Sig2.New[] {
//             Sig2.NewCollapsed(1, 2), Sig2.NewLoop(2, -1)
//         }),
//         Sig2.NewExpanded(0, Sig2.New[] {
//             Sig2.NewCollapsed(1, 1), Sig2.NewLoop(2, -1)
//         })
//         );
//         Assert.That(signatures, Is.EqualTo("[[2,-1],[1,-1]]"));
//     }
//
//     [Test]
//     public void NestedExpandedSig()
//     {
//         var signatures = GetSignature(
//             Sig2.NewExpanded(0, Sig2.New[] {
//             Sig2.NewExpanded(1, Sig2.New[] {
//                 Sig2.NewCollapsed(2, 2)
//             })
//         }),
//         Sig2.NewExpanded(0, Sig2.New[] {
//             Sig2.NewExpanded(1, Sig2.New[] {
//                 Sig2.NewCollapsed(2, 1)
//             })
//         })
//         );
//         Assert.That(signatures, Is.EqualTo("[[[2]],[[1]]]"));
//     }
//
//     [Test]
//     public void MultipleCollapsedSig_in_order()
//     {
//         var signatures = GetSignature(Sig2.NewCollapsed(1, 3), Sig2.NewCollapsed(2, 1), Sig2.NewCollapsed(3, 2));
//         Assert.That(signatures, Is.EqualTo("[3,2,1]"));
//     }
//
//     [Test]
//     public void ExpandedSig_with_identical_elements()
//     {
//         var signatures = GetSignature(
//             Sig2.NewExpanded(0, Sig2.New[] {
//             Sig2.NewCollapsed(1, 2), Sig2.NewCollapsed(2, 1)
//         }),
//         Sig2.NewExpanded(0, Sig2.New[] {
//             Sig2.NewCollapsed(1, 2), Sig2.NewCollapsed(2, 1)
//         })
//         );
//         Assert.That(signatures, Is.EqualTo("[[2,1],[2,1]]"));
//     }
//
//     [Test]
//     public void ExpandedSig_with_different_LinkSig_levels()
//     {
//         var signatures = GetSignature(
//             Sig2.NewExpanded(0, Sig2.New[] {
//             Sig2.NewLoop(1, -2), Sig2.NewLoop(2, -1)
//         }),
//         Sig2.NewExpanded(0, Sig2.New[] {
//             Sig2.NewLoop(1, -1), Sig2.NewLoop(2, -2)
//         })
//         );
//         Assert.That(signatures, Is.EqualTo("[[-2,-1],[-1,-2]]"));
//     }
//
//     [Test]
//     public void EmptyExpandedSig_and_CollapsedSig()
//     {
//         Assert.That(() => GetSignature(Sig2.NewExpanded(0, Sig2.New[] {
//         }), Sig2.NewCollapsed(1, 0)),
//         Is.EqualTo("[0,[]]"));
//     }
}