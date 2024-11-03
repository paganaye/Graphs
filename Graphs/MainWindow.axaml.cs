using System;
using System.Threading.Tasks;

namespace Graphs;

using Avalonia.Controls;
using Avalonia.Interactivity;

public partial class MainWindow : Window
{
    private Graph graph1;
    private Graph graph2;

    public MainWindow()
    {
        InitializeComponent();
        graph1 = EdgeList.Deserialize(9, "(1,2),(3,5),(6,8),(7,8),(4,9),(7,9)");
        graph2 = graph1.shuffled();
        GraphControl1.SetGraph(graph1);
        GraphControl2.SetGraph(graph2);
        CompareButton.Click += OnCompareButtonClick;
        CompareSignatures();
    }

    void OnCompareButtonClick(object sender, RoutedEventArgs e)
    {
        CompareSignatures();
    }

    private async void CompareSignatures()
    {
        CompareResult.Text = "...";
        await Task.Delay(1); // Allow the UI to update

        // Step 1: Check if the graphs have the same number of nodes
        if (graph1.NodeCount != graph2.NodeCount)
        {
            CompareResult.Text = "The graphs have different numbers of nodes and cannot be compared directly.";
            return;
        }

        // Step 2: Check if the graphs are identical
        bool areIdentical = GraphComparer.Compare(graph1, graph2);
        if (areIdentical)
        {
            CompareResult.Text = "The graphs are identical!";
            return;
        }

        // Step 3: Calculate and display the signature for each graph, with timing
        var stopwatch1 = System.Diagnostics.Stopwatch.StartNew();
        var g1s = graph1.CalculateSignature();
        stopwatch1.Stop();
        var stopwatch2 = System.Diagnostics.Stopwatch.StartNew();
        var g2s = graph2.CalculateSignature();
        stopwatch2.Stop();
        var sameSignature = g1s == g2s;

        var signatureTimeMessage =
            $"Signature calculations took {stopwatch1.ElapsedMilliseconds} and {stopwatch2.ElapsedMilliseconds} ms.";

        // Step 4: Check isomorphism
        string isomorphic;
        var areIsomorphic = SimpleIsomorphic.AreIsomorphic(graph1, graph2);
        isomorphic =
            areIsomorphic == null
                ? ""
                : $"The simple isomorphism algorithm says the graphs are {(areIsomorphic.Value ? "isomorphic" : "not isomorphic")}.\n";

        // Step 5: Display the final result
        CompareResult.Text = $"{isomorphic}{signatureTimeMessage}\n" +
                             (sameSignature
                                 ? "✅ The signatures match."
                                 : "❌ The signatures are not identical.");
    }

    private void OnGraph1Changed(object? sender, Graph g)
    {
        graph1 = g;
        CompareSignatures();
    }

    private void OnGraph2Changed(object? sender, Graph g)
    {
        graph2 = g;
        CompareSignatures();
    }
}