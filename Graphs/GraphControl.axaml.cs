namespace Graphs;

using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;

public partial class GraphControl : UserControl
{
    private Graph? _graph;

    public event EventHandler<Graph>? GraphChanged;

    public GraphControl()
    {
        InitializeComponent();

        // Initialiser le graphe par défaut
        _ = UpdateGraph();

        // Souscrire les événements des boutons
        RandomGraphButton.Click += OnRandomGraphButtonClick;
        ShuffleButton.Click += OnShuffleButtonClick;
        NodeCountControl.ValueChanged += OnNodeCountChanged;
        EdgeListTextBox.TextChanged += OnEdgeListTextBoxChanged;
        Graph6TextBox.TextChanged += Graph6TextBoxOnTextChanged;
        AdjacencyTextBox.TextChanged += AdjacencyTextBoxOnTextChanged;
    }


    public async Task SetGraph(Graph graph)
    {
        if (!graph.Equals(_graph))
        {
            _graph = graph;
            await UpdateGraph();
            GraphChanged?.Invoke(this, _graph);
        }
    }

    private void OnRandomGraphButtonClick(object sender, RoutedEventArgs e)
    {
        CreateRandomGraph(((double)(FillControl.Value ?? decimal.Zero)) / 100.0);
    }

    private void OnShuffleButtonClick(object sender, RoutedEventArgs e)
    {
        ShuffleGraph();
    }

    private void OnNodeCountChanged(object sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_graph == null) return;
        if (e.NewValue is { } newCount && newCount != _graph.NodeCount)
        {
            SetGraph(GraphResizer.Resize(_graph, (int)newCount));
        }
    }

    private void CreateRandomGraph(double fill)
    {
        if (_graph == null) return;
        SetGraph(GraphRandomizer.RandomGraph(_graph.NodeCount, fill));
    }

    private void ShuffleGraph()
    {
        if (_graph == null) return;
        SetGraph(GraphShuffler.Shuffled(_graph));
    }

    private void OnEdgeListTextBoxChanged(object? sender, TextChangedEventArgs e)
    {
        if (_graph == null) return;
        try
        {
            var edgeList = EdgeListTextBox.Text ?? "";
            var newGraph = EdgeList.Deserialize((int)(NodeCountControl.Value ?? Decimal.Zero), edgeList);
            SetGraph(newGraph);
        }
        catch
        {
            // ignored
        }
    }

    private void AdjacencyTextBoxOnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_graph == null) return;
        try
        {
            var text = AdjacencyTextBox.Text ?? "";
            if (text.Length == 0) return;
            var newGraph = AdjacencyList.Deserialize(text);
            SetGraph(newGraph);
        }
        catch
        {
            // ignored
        }
    }

    private void Graph6TextBoxOnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_graph == null) return;
        try
        {
            var text = Graph6TextBox.Text ?? "";
            if (text.Length == 0) return;
            var newGraph = Graph6.Deserialize(text);
            SetGraph(newGraph);
        }
        catch
        {
            // ignored
        }
    }

    private async Task UpdateGraph()
    {
        if (_graph == null) return;

        Controls.IsEnabled = false;
        await Task.Delay(1);
        try
        {
            AdjacencyTextBox.Text = AdjacencyList.Serialize(_graph);
            EdgeListTextBox.Text = EdgeList.Serialize(_graph);
            Graph6TextBox.Text = Graph6.Serialize(_graph);
            NodeCountControl.Value = _graph.NodeCount;
            await Task.Delay(1);
            GraphView.Content = new GraphView(_graph);
            await Task.Delay(1);
            GraphSignatureTextBox.Text = new GraphSignature(_graph).ToString();
            GraphSignatureTextBox.IsEnabled = true;
        }
        finally
        {
            Controls.IsEnabled = true;
        }
    }
}