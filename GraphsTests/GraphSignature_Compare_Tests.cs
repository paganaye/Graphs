using Graphs;

namespace GraphSignature_Tests;

[TestFixture]
public class GraphSignature_Compare_Tests
{
    [Test]
    public void compare_two_simple_signatures()
    {
        var graph = new Graph(5);
        graph.SetEdge(0, 1, true);
        graph.SetEdge(2, 3, true);
        graph.SetEdge(3, 4, true);

        var graphSignature = new GraphSignature(graph);

        Assert.That(graphSignature.UnsortedNodeSignatures[0].ToString(), Is.EqualTo("[[-1]]"),
            "Simple node pair");

        Assert.That(graphSignature.UnsortedNodeSignatures[1].ToString(), Is.EqualTo("[[-1]]"),
            "Simple node pair");


    }
}