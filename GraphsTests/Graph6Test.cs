// using Graphs;
//
// namespace GraphsTests;
//
// [TestFixture]
// [TestOf(typeof(Graph6))]
// public class Graph6Test
// {
//
//     // [Test]
//     // public void TestSimpleEmptyC6Graph()
//     // {
//     //     var c6 = "C?";
//     //     var newGraph = Graph6.Deserialize(c6);
//     //     var newC6 = Graph6.Serialize(newGraph);
//     //     Assert.That(newC6,Is.EqualTo(c6));
//     // }
//     //
//     // [Test]
//     // public void TestSimpleCgGraph()
//     // {
//     //     var c6 = "D{";
//     //     var newGraph = Graph6.Deserialize(c6);
//     //     var newC6 = Graph6.Serialize(newGraph);
//     //     Assert.That(newC6,Is.EqualTo(c6));
//     // }
//     
//
//     [Test]
//     public void TestSimpleEmptyC6Graph()
//     {
//         var c6 = "C?";
//         var newGraph = Graph6.Deserialize(c6);
//         var newC6 = Graph6.Serialize(newGraph);
//         Assert.That(newC6,Is.EqualTo(c6));
//     }
//
//     [Test]
//     public void TestSimpleCgGraph()
//     {
//         var c6 = "D{";
//         var newGraph = Graph6.Deserialize(c6);
//         var newC6 = Graph6.Serialize(newGraph);
//         Assert.That(newC6,Is.EqualTo(c6));
//     }    
// }