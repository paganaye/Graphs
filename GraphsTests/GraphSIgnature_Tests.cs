using Graphs;

namespace GraphsTests;

[TestFixture]
public class GraphSignatureTests
{
    [Test]
    public void graph_with_two_nodes_and_one_edge()
    {
        var graph = new GraphBuilder(2);
        graph.SetEdge(0, 1, true);
        var signature = new GraphSignature(graph.Build());
        Assert.That(signature.ToString(), Is.EqualTo("![[],[]]"));
    }

    [Test]
    public void graph_with_three_nodes_and_two_edges()
    {
        var graph = new GraphBuilder(5);
        graph.SetEdge(0, 1, true); // A-B
        graph.SetEdge(1, 2, true); // B-C

        var signature = new GraphSignature(graph.Build());

        // Assert.That(signature.UnsortedNodeSignatures[0].DebugSig(), Is.EqualTo("ANeighbours(1)[BNeighbours(2)[A:Loop(-1),CNeighbours(1)[B:Loop(-1)]]]"));
        // Assert.That(signature.UnsortedNodeSignatures[1].DebugSig(), Is.EqualTo("BNeighbours(2)"));
        // Assert.That(signature.UnsortedNodeSignatures[2].DebugSig(), Is.EqualTo("CNeighbours(1)[BNeighbours(2)[C:Loop(-1),ANeighbours(1)[B:Loop(-1)]]]"));

        var cmp = new SigComparer();
        Assert.That(cmp.Compare(signature.UnsortedNodeSignatures[0], signature.UnsortedNodeSignatures[2]),
            Is.EqualTo(0));

        // then this should be sorted
        Assert.That(signature.ToString(), Is.EqualTo("[2,[],[],[[-1,[-1]]],[[-1,[-1]]]]"));
    }

    [Test]
    public void graph_with_multiple_edges()
    {
        var graph = new GraphBuilder(5);
        graph.SetEdge(0, 1, true); // A-B
        graph.SetEdge(0, 2, true); // A-C
        graph.SetEdge(1, 2, true); // B-C
        graph.SetEdge(0, 3, true); // A-D
        graph.SetEdge(3, 4, true); // D-E

        var signature = new GraphSignature(graph.Build());


        // Assert.That(signature.UnsortedNodeSignatures[0].DebugSig(), Is.EqualTo("ANeighbours(3)"));
        // Assert.That(signature.UnsortedNodeSignatures[4].DebugSig(), Is.EqualTo("ENeighbours(1)"));

        Assert.That(signature.ToString(), Is.EqualTo("[3,1,[3,1],[[-1,[-2,-1,[-1,[-1]]]],[-1,[-2,-1],[-1,[-1]]]]]"));
    }

    [Test]
    public void FnY_and_FvY()
    {
        //   FnY?? is not isomorphic with FvY?? but they have the same signature.
        //      Signature: [4,2,1,0,[4,3,3]]
        var g1 = Graph6.Deserialize("FnY??");
        var g2 = Graph6.Deserialize("FvY??");
        var g1S = g1.CalculateSignature();
        var g2S = g2.CalculateSignature();

        Assert.That(g1S != g2S);
    }

    [Test]
    public void Frtc_and_Fltc()
    {
        // Frtc? is not isomorphic with Fltc? but they have the same signature.
        // Signature: [2,1,[4,4,3],[4,3,3],[4,3,2],[4,3,3,2],[4,3,3,1]]

        var g1 = Graph6.Deserialize("Frtc?");
        var g2 = Graph6.Deserialize("Fltc?");
        var g1S = g1.CalculateSignature();
        var g2S = g2.CalculateSignature();

        Assert.That(g1S != g2S);
    }
}