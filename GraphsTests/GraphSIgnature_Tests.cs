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
      Assert.That(signature.ToString(),Is.EqualTo("![[],[]]"));
   }
   
   [Test]
   public void graph_with_three_nodes_and_two_edges()
   {
      var graph = new GraphBuilder(5);
      graph.SetEdge(0, 1, true); // A-B
      graph.SetEdge(1, 2, true); // B-C
    
      var signature = new GraphSignature(graph.Build());
    
      Assert.That(signature.UnsortedNodeSignatures[0].DebugSig(), Is.EqualTo("ANeighbours(1)[BNeighbours(2)[A:Loop(1),CNeighbours(1)[B:Loop(2)]]]"));
      Assert.That(signature.UnsortedNodeSignatures[1].DebugSig(), Is.EqualTo("BNeighbours(2)"));
      Assert.That(signature.UnsortedNodeSignatures[2].DebugSig(), Is.EqualTo("CNeighbours(1)[BNeighbours(2)[C:Loop(1),ANeighbours(1)[B:Loop(2)]]]"));

      var cmp = new SigComparer();
      Assert.That(cmp.Compare(signature.UnsortedNodeSignatures[0],signature.UnsortedNodeSignatures[2]), Is.EqualTo(0));

      // then this should be sorted
      Assert.That(signature.ToString(), Is.EqualTo("[2,[],[],[[-1,[-2]]],[[-1,[-2]]]]"));
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

      
      Assert.That(signature.UnsortedNodeSignatures[0].DebugSig(), Is.EqualTo("ANeighbours(3)"));
      Assert.That(signature.UnsortedNodeSignatures[1].DebugSig(), Is.EqualTo("BNeighbours(2)[CNeighbours(2)[B:Loop(1),ANeighbours(3)[B:Loop(1),C:Loop(2),DNeighbours(2)[A:Loop(3),ENeighbours(1)[D:Loop(4)]]]],ANeighbours(3)[B:Loop(1),CNeighbours(2)[B:Loop(1),A:Loop(2)],DNeighbours(2)[A:Loop(2),ENeighbours(1)[D:Loop(3)]]]]"));
      Assert.That(signature.UnsortedNodeSignatures[2].DebugSig(), Is.EqualTo("CNeighbours(2)[BNeighbours(2)[C:Loop(1),ANeighbours(3)[C:Loop(1),B:Loop(2),DNeighbours(2)[A:Loop(3),ENeighbours(1)[D:Loop(4)]]]],ANeighbours(3)[C:Loop(1),BNeighbours(2)[C:Loop(1),A:Loop(2)],DNeighbours(2)[A:Loop(2),ENeighbours(1)[D:Loop(3)]]]]"));
      Assert.That(signature.UnsortedNodeSignatures[3].DebugSig(), Is.EqualTo("DNeighbours(2)[ANeighbours(3),ENeighbours(1)]"));
      Assert.That(signature.UnsortedNodeSignatures[4].DebugSig(), Is.EqualTo("ENeighbours(1)"));

      Assert.That(signature.ToString(), Is.EqualTo("[3,1,[3,1],[[-1,[-1,-2,[-3,[-4]]]],[-1,[-1,-2],[-2,[-3]]]]]"));
            
   }

}   