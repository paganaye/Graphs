using System;
using System.Threading.Tasks;

namespace Graphs;

using Avalonia.Controls;
using Avalonia.Interactivity;

public partial class MainWindow : Window
{
    private readonly Graph graph1;
    private readonly Graph graph2;

    public MainWindow()
    {
        InitializeComponent();
        graph1 = new Graph(5);
        graph2 = new Graph(5);
        GraphControl1.SetGraph(graph1);
        GraphControl2.SetGraph(graph2);
        CompareButton.Click += OnCompareButtonClick;
    }

    private async void OnCompareButtonClick(object sender, RoutedEventArgs e)
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
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var g1s = graph1.Signature();
        stopwatch.Stop();
        var g2s = graph2.Signature();
        var sameSignature = g1s == g2s;

        var signatureTimeMessage = $"Signature calculation for graph1 took {stopwatch.ElapsedMilliseconds} ms.";

        // Step 4: Check isomorphism
        string isomorphic;
        try
        {
            isomorphic =
                $"\nThe simple isomorphism algorithm says the graphs are {(SimpleIsomorphic.Compare(graph1, graph2) ? "isomorphic" : "not isomorphic")}.";
        }
        catch (Exception ex)
        {
            // ignored
            isomorphic = "";
        }

        // Step 5: Display the final result
        CompareResult.Text = $"{signatureTimeMessage}\n" +
                             (sameSignature
                                 ? "✅ The signatures match."
                                 : "❌ The signatures are not identical.") + isomorphic;
    }
}