using System.Collections.Generic;

namespace Graphs;

using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using Avalonia.Interactivity;

public partial class Research : UserControl
{
    private Graph graph1;

    public Research()
    {
        InitializeComponent();
        graph1 = EdgeList.Deserialize(9, "(1,2),(3,5),(6,8),(7,8),(4,9),(7,9)");
        GraphControl1.SetGraph(graph1);
    }

    void OnCompareButtonClick(object sender, RoutedEventArgs e)
    {
    }


    private void OnGraph1Changed(object? sender, Graph g)
    {
        graph1 = g;
    }

    private void CompareButton_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private List<(int, int)> edges = [];
    private Int64 configCount = 0;
    private Int64 config = 0;


    private void Button3_OnClick(object? sender, RoutedEventArgs e) => BruteForce(3, Button3Text);
    private void Button4_OnClick(object? sender, RoutedEventArgs e) => BruteForce(4, Button4Text);
    private void Button5_OnClick(object? sender, RoutedEventArgs e) => BruteForce(5, Button5Text);
    private void Button6_OnClick(object? sender, RoutedEventArgs e) => BruteForce(6, Button6Text);
    private void Button7_OnClick(object? sender, RoutedEventArgs e) => BruteForce(7, Button7Text);
    private void Button8_OnClick(object? sender, RoutedEventArgs e) => BruteForce(8, Button8Text);

    private void Button6S_OnClick(object? sender, RoutedEventArgs e) => SemiBruteForce(3, 3, Button6SText);
    private void Button7S_OnClick(object? sender, RoutedEventArgs e) => SemiBruteForce(3, 4, Button7SText);
    private void Button8S_OnClick(object? sender, RoutedEventArgs e) => SemiBruteForce(4, 4, Button8SText);
    private void Button9S_OnClick(object? sender, RoutedEventArgs e) => SemiBruteForce(4, 5, Button9SText);
    private void Button10S_OnClick(object? sender, RoutedEventArgs e) => SemiBruteForce(5, 5, Button10SText);


    private async Task<Dictionary<string, Graph>> BruteForce(int nodeCount, TextBlock textBlock)
    {
        var graphBuilder = new GraphBuilder(nodeCount);
        Info.Text = $"Brute force {nodeCount}...";
        edges = BuildEdges(nodeCount);

        configCount = 1 << edges.Count;
        DateTime lastDisplayed = DateTime.UnixEpoch;
        Graph? graph = null;
        ProgressBar1.IsVisible = true;
        Dictionary<string, Graph> signatures = new();

        async Task Display()
        {
            await GraphControl1.SetGraph(graph);
            lastDisplayed = DateTime.Now;
            textBlock.Text = $"{signatures.Count:N0} signatures found in {config:N0} graphs";
            ProgressBar1.Value = (double)config / configCount * 100;
        }

        for (config = 0; config < configCount; config++)
        {
            graphBuilder.Clear();
            for (int i = 0; i < edges.Count; i++)
            {
                if ((config & (1 << i)) != 0)
                {
                    var (node1, node2) = edges[i];
                    graphBuilder.SetEdge(node1, node2, true);
                }
            }

            graph = graphBuilder.Build();
            var sig = graph.CalculateSignature();
            if (!signatures.ContainsKey(sig)) signatures.Add(sig, graph);
            if ((DateTime.Now - lastDisplayed).TotalMilliseconds > 100)
            {
                await Display();
            }
        }

        if (graph != null)
        {
            await Display();
            ProgressBar1.IsVisible = false;
        }

        Info.Text = $"Brute force {nodeCount} nodes graphs gives us {signatures.Count} distinct signatures.";

        return signatures;
    }

    private async Task<Dictionary<string, Graph>> SemiBruteForce(int leftNodeCount, int rightNodeCount,
        TextBlock textBlock)
    {
        var leftSignatures = await BruteForce(leftNodeCount, Info);
        var rightSignatures = await BruteForce(rightNodeCount, Info);

        int nodeCount = leftNodeCount + rightNodeCount;

        var graphBuilder = new GraphBuilder(nodeCount);

        Info.Text = $"Semi brute force {nodeCount}...";
        edges = BuildEdges(nodeCount);

        configCount = leftSignatures.Count * rightSignatures.Count;

        DateTime lastDisplayed = DateTime.UnixEpoch;
        Graph? graph = null;
        ProgressBar1.IsVisible = true;
        Dictionary<string, Graph> signatures = new();

        async Task Display()
        {
            await GraphControl1.SetGraph(graph);
            lastDisplayed = DateTime.Now;
            textBlock.Text = $"{signatures.Count:N0} signatures found in {configCount:N0} graphs";
            ProgressBar1.Value = (double)config / configCount * 100;
        }

        var innerEdges = BuildInnerEdges(leftNodeCount, rightNodeCount);
        var config1Count = leftSignatures.Count * rightSignatures.Count;
        var innerConfigCount = 1 << innerEdges.Count;
        var rightGraphOffset = leftNodeCount;

        configCount = config1Count * innerConfigCount;
        config = 0;

        foreach (var leftSig in leftSignatures)
        {
            CopyEdges(leftSig.Value, graphBuilder, 0);
            foreach (var rightSig in rightSignatures)
            {
                CopyEdges(rightSig.Value, graphBuilder, rightGraphOffset);
                for (var innerConfig = 0; innerConfig < innerConfigCount; innerConfig++)
                {
                    for (int i = 0; i < innerEdges.Count; i++)
                    {
                        var isSet = (innerConfig & (1 << i)) != 0;
                        var (node1, node2) = innerEdges[i];
                        graphBuilder.SetEdge(node1, node2, isSet);
                    }

                    graph = graphBuilder.Build();
                    var sig = graph.CalculateSignature();
                    if (!signatures.ContainsKey(sig)) signatures.Add(sig, graph);


                    if ((DateTime.Now - lastDisplayed).TotalMilliseconds > 100)
                    {
                        await Display();
                    }
                    config += 1;
                }
            }
        }


        if (graph != null)
        {
            await Display();
            ProgressBar1.IsVisible = false;
        }

        Info.Text =
            $"Semi brute force of {leftNodeCount} and {rightNodeCount} nodes graphs gives us {signatures.Count} distinct signatures.";

        return signatures;
    }

    private void CopyEdges(Graph graph, GraphBuilder graphBuilder, int offset)
    {
        for (int j = 1; j < graph.NodeCount; j++)
        {
            for (int i = 0; i < j; i++)
            {
                var hasEdge = graph.HasEdge(i, j);
                graphBuilder.SetEdge(i + offset, j + offset, hasEdge);
            }
        }
    }

    List<(int, int)> BuildEdges(int nodeCount)
    {
        var result = new List<(int, int)>();
        for (var j = 0; j < nodeCount; j++)
        {
            for (var i = 0; i < j; i++) result.Add((i, j));
        }

        return result;
    }

    List<(int, int)> BuildInnerEdges(int leftCount, int rightCount)
    {
        var result = new List<(int, int)>();
        var rightOffset = leftCount;
        for (var leftIndex = 0; leftIndex < leftCount; leftIndex++)
        {
            for (var rightIndex = 0; rightIndex < rightCount; rightIndex++)
            {
                result.Add((leftIndex, rightIndex + rightOffset));
            }
        }

        return result;
    }
}