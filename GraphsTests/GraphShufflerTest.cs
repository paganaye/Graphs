using Graphs;

namespace GraphsTests;

[TestFixture]
[TestOf(typeof(GraphShuffler))]
public class GraphShufflerTest
{
    [Test]
    public void SimpleShuffle()
    {
        var graph = EdgeList.Deserialize(5, "(1,2),(2,3),(3,4),(2,5)");
        Assert.That(EdgeList.Serialize(graph.shuffled(1)), Is.EqualTo("(1,2),(2,5),(3,5),(4,5)"));
        Assert.That(EdgeList.Serialize(graph.shuffled(2)), Is.EqualTo("(1,4),(2,4),(3,4),(1,5)"));
        Assert.That(EdgeList.Serialize(graph.shuffled(3)), Is.EqualTo("(3,4),(1,5),(2,5),(4,5)"));
        Assert.That(EdgeList.Serialize(graph.shuffled(4)), Is.EqualTo("(1,3),(2,3),(1,4),(3,5)"));
        Assert.That(EdgeList.Serialize(graph.shuffled(5)), Is.EqualTo("(1,2),(1,5),(3,5),(4,5)"));
    }
}