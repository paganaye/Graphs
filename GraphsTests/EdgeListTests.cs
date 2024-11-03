using Graphs;

namespace GraphsTests;

[TestFixture]
[TestOf(typeof(EdgeList))]
public class EdgeListTests
{
    [Test]
    public void TestSimpleEdgeListGraph()
    {
        var list = "(1,2),(3,5),(4,9),(7,8),(7,9)";
        var newGraph = EdgeList.Deserialize(10, list);
        var newList = EdgeList.Serialize(newGraph);
        Assert.That(newList, Is.EqualTo(list));
    }

    [Test]
    public void SerializeTwoEdgesGraph()
    {
        var builder = new GraphBuilder(5);
        builder.SetEdge(0, 1, true);
        builder.SetEdge(3, 4, true);

        var list = EdgeList.Serialize(builder.Build());
        Assert.That(list, Is.EqualTo("(1,2),(4,5)"));
    }

    [Test]
    public void DeserializeTwoEdgesGraph()
    {
        var graph = EdgeList.Deserialize(5, "(1,2),(4,5)");
        Assert.That(graph.NodeCount, Is.EqualTo(5));
        Assert.That(graph.HasEdge(0, 1), Is.True);
        Assert.That(graph.HasEdge(3, 4), Is.True);
        Assert.That(graph.ActiveEdges, Is.EqualTo(2));
    }
}