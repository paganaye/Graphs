namespace Graphs;

internal static class GraphComparer
{
    public static bool Compare(Graph graph1, Graph graph2)
    {
        if (graph1.NodeCount != graph2.NodeCount) return false;

        for (int j = 1; j < graph1.NodeCount; j++)
        {
            for (int i = 0; i < j; i++)
            {
                if (graph1.HasEdge(i, j) != graph2.HasEdge(i, j))
                {
                    return false;
                }
            }
        }

        return true;
    }
}