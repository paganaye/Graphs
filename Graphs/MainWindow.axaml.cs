namespace Graphs;

using Avalonia.Controls;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

public partial class MainWindow : Window
{
    private Graph _graph;

    public MainWindow()
    {
        InitializeComponent();
        _graph = new Graph(12);
        // Subscribe buttons to their events
        RandomGraphButton.Click += OnRandomGraphButtonClick!;
        ShuffleButton.Click += OnShuffleButtonClick!;
        NodeCountControl.ValueChanged += OnNodeCountChanged!;
        CreateRandomGraph();
    }


    private void OnRandomGraphButtonClick(object sender, RoutedEventArgs e)
    {
        CreateRandomGraph();
    }

    private void OnShuffleButtonClick(object sender, RoutedEventArgs e)
    {
        ShuffleGraph();
    }

    private void CreateRandomGraph()
    {
        GraphRandomizer.FillRandomAdjacencies(_graph); // Fill the graph with random connections
        NodeCountControl.Value = _graph.NodeCount;
        UpdateGraph();
    }

    private void ShuffleGraph()
    {
        GraphShuffler.ShuffleGraph(_graph); // Shuffle existing connections
        UpdateGraph();
    }

    private void OnNodeCountChanged(object sender, NumericUpDownValueChangedEventArgs e)
    {
        if (e.NewValue is { } newCount && newCount != _graph.NodeCount)
        {
            _graph = GraphCloner.CopyResizeGraph(_graph, (int)newCount);
            UpdateGraph();
        }
    }

    private void OnGraph6TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (Graph6TextBox.Text != null)
            {
                _graph = Graph6.Deserialize(Graph6TextBox.Text);
                UpdateGraph();
            }
        }
        catch (Exception)
        {
            // Gérer les erreurs de désérialisation si nécessaire
        }
    }


    private void UpdateGraph()
    {
        GraphView1.Content = new GraphView(_graph);
        Graph6TextBox.Text=  Graph6.Serialize(_graph);
        NodeCountControl.Value = _graph.NodeCount;
    }
}