using Avalonia.Controls;
using System;
using Avalonia;

namespace Graphs
{
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

        private void OnRandomGraphButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            CreateRandomGraph();
        }

        private void OnShuffleButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShuffleGraph();
        }

        private void CreateRandomGraph()
        {
            GraphRandomizer.FillRandomAdjacencies(_graph); // Fill the graph with random connections
            NodeCountControl.Value = _graph.NodeCount;
            GraphView1.Content = new GraphView(_graph);
        }

        private void ShuffleGraph()
        {
            GraphShuffler.ShuffleGraph(_graph); // Shuffle existing connections
            GraphView1.Content = new GraphView(_graph); // Update the display
        }

        private void OnNodeCountChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            if (e.NewValue is { } newCount && newCount!=_graph.NodeCount)
            {
                _graph = GraphCloner.CopyResizeGraph(_graph, (int)newCount);
                GraphView1.Content = new GraphView(_graph); 
            }
        }

    }
}