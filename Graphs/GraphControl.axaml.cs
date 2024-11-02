using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Graphs
{
    public partial class GraphControl : UserControl
    {
        private Graph _graph;

        public GraphControl()
        {
            InitializeComponent();

            // Initialiser le graphe par défaut

            // Souscrire les événements des boutons
            RandomGraphButton.Click += OnRandomGraphButtonClick;
            ShuffleButton.Click += OnShuffleButtonClick;
            NodeCountControl.ValueChanged += OnNodeCountChanged;
            Graph6TextBox.TextChanged += OnGraph6TextBoxChanged;
            UpdateGraph();
        }


        public void SetGraph(Graph graph)
        {
            _graph = graph;
            UpdateGraph();
        }

        private void OnRandomGraphButtonClick(object sender, RoutedEventArgs e)
        {
            CreateRandomGraph();
        }

        private void OnShuffleButtonClick(object sender, RoutedEventArgs e)
        {
            ShuffleGraph();
        }

        private void OnNodeCountChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            if (e.NewValue is { } newCount && newCount != _graph.NodeCount)
            {
                GraphResizer.SetNodeCount(_graph, (int)newCount);
                UpdateGraph();
            }
        }

        private void CreateRandomGraph()
        {
            GraphRandomizer.FillRandomAdjacencies(_graph);
            UpdateGraph();
        }

        private void ShuffleGraph()
        {
            GraphShuffler.ShuffleGraph(_graph);
            UpdateGraph();
        }

        private void OnGraph6TextBoxChanged(object? sender, TextChangedEventArgs e)
        {
            try
            {
                Graph6.Deserialize(this._graph, Graph6TextBox.Text ?? "");
                UpdateGraph();
            }
            catch
            {
                // ignored
            }
        }


        private void UpdateGraph()
        {
            if (_graph == null) return;
            GraphView.Content = new GraphView(_graph);
            Graph6TextBox.Text = Graph6.Serialize(_graph);
            NodeCountControl.Value = _graph.NodeCount;
        }
    }
}