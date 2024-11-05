using Graphs;

namespace GraphsTests;

[TestFixture]
[TestOf(typeof(Graph6))]
public class Graph6Test
{
    // [Test]
    // public void TestSimpleEmptyC6Graph()
    // {
    //     var c6 = "C?";
    //     var newGraph = Graph6.Deserialize(c6);
    //     var newC6 = Graph6.Serialize(newGraph);
    //     Assert.That(newC6,Is.EqualTo(c6));
    // }
    //
    // [Test]
    // public void TestSimpleCgGraph()
    // {
    //     var c6 = "D{";
    //     var newGraph = Graph6.Deserialize(c6);
    //     var newC6 = Graph6.Serialize(newGraph);
    //     Assert.That(newC6,Is.EqualTo(c6));
    // }

    // https://houseofgraphs.org/graphs/26923
    [Test]
    public void TestSimplestC6Graph()
    {
        var test1 = new
        {
            url = "https://houseofgraphs.org/",
            adjacencyList = "1: 2\n2: 1\n3:\n",
            Graph6 = "B_"
        };
        var g = AdjacencyList.Deserialize(test1.adjacencyList);
        Assert.That(g.HasEdge(0, 1), Is.True);
        Assert.That(g.HasEdge(0, 2), Is.False);
        Assert.That(g.HasEdge(1, 2), Is.False);
        var g6 = Graph6.Serialize(g);
        Assert.That(g6, Is.EqualTo(test1.Graph6.ToString()));
    }

    [Test]
    public void TestSecondSimplestC6Graph()
    {
        var test1 = new
        {
            url = "https://houseofgraphs.org/",
            adjacencyList = "1: 2\n2: 1 3\n3: 2\n",
            Graph6 = "Bg"
        };
        var g = AdjacencyList.Deserialize(test1.adjacencyList);
        Assert.That(g.HasEdge(0, 1), Is.True);
        Assert.That(g.HasEdge(0, 2), Is.False);
        Assert.That(g.HasEdge(1, 2), Is.True);
        var g6 = Graph6.Serialize(g);
        Assert.That(g6, Is.EqualTo(test1.Graph6.ToString()));
    }


    [Test]
    public void TestSimpleC6Graph()
    {
        var test1 = new
        {
            url = "https://houseofgraphs.org/graphs/26923",
            adjacencyList = "1:6\n 2:7\n 3:4 5\n 4:3 6\n 5: 3 7\n 6:1 4 7\n 7:2 5 6",
            G6 = "F@IQW"
        };
        var g = AdjacencyList.Deserialize(test1.adjacencyList);
        Assert.That(g.HasEdge(0, 5), Is.True);
        Assert.That(g.HasEdge(1, 6), Is.True);
        Assert.That(g.HasEdge(2, 3), Is.True);
        Assert.That(g.HasEdge(2, 4), Is.True);
        Assert.That(g.HasEdge(3, 2), Is.True);
        Assert.That(g.HasEdge(3, 5), Is.True);
        Assert.That(g.HasEdge(4, 2), Is.True);
        Assert.That(g.HasEdge(4, 6), Is.True);
        Assert.That(g.HasEdge(5, 0), Is.True);
        Assert.That(g.HasEdge(5, 3), Is.True);
        Assert.That(g.HasEdge(5, 6), Is.True);
        Assert.That(g.HasEdge(6, 1), Is.True);
        Assert.That(g.HasEdge(6, 4), Is.True);
        Assert.That(g.HasEdge(6, 5), Is.True);

        var g2 = Graph6.Deserialize(test1.G6);
        Assert.That(g, Is.EqualTo(g2));
        var g6 = Graph6.Serialize(g);
        Assert.That(g6, Is.EqualTo(test1.G6));
    }
}