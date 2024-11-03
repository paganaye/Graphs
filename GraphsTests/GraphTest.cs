using Graphs;

namespace GraphsTests;

[TestFixture]
[TestOf(typeof(Graph))]
public class GraphTest
{
    [Test]
    public void EmptyGraph()
    {
        var builder = new GraphBuilder(10);
        var g1 = new Graph(builder);
        Assert.That(g1.NodeCount, Is.EqualTo(10));
        Assert.That(g1.ActiveEdges, Is.EqualTo(0));
    }

    [Test]
    public void TwoEdgesGraph()
    {
        var builder = new GraphBuilder(5);
        builder.SetEdge(0, 1, true);
        builder.SetEdge(3, 4, true);
        
        var g1 = new Graph(builder);
        Assert.That(g1.NodeCount, Is.EqualTo(5));
        Assert.That(g1.ActiveEdges, Is.EqualTo(2));
    }
}