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
        await Task.Delay(1); // Laisser le temps à l'interface de se mettre à jour

        if (graph1.NodeCount != graph2.NodeCount)
        {
            CompareResult.Text = "The graphs have different numbers of nodes and cannot be compared directly.";
            return;
        }

        bool areIdentical = GraphComparer.Compare(graph1, graph2);

        if (areIdentical)
        {
            CompareResult.Text = "The graphs are identical!";
        }
        else
        {
            var g1s = graph1.Signature();
            var g2s = graph2.Signature();
            var sameSignature = g1s == g2s;

            try
            {
                if (SimpleIsomorphic.Compare(graph1, graph2))
                {
                    CompareResult.Text = "The graphs are isomorphic" +
                                         (sameSignature
                                             ? " and the signatures match."
                                             : " but the signatures are not identical.");
                }
                else
                {
                    CompareResult.Text = "The graphs are different " +
                                         (sameSignature
                                             ? " but the signatures match."
                                             : " and the signatures are not identical.");
                }
            }
            catch (Exception ex)
            {
                CompareResult.Text = ex.Message +
                                     (sameSignature
                                         ? " - \u2705 The signatures match."
                                         : " - \u274c The signatures are not identical.");
            }
        }
    }
}