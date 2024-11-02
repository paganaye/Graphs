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
            graph1 = new Graph(12);
            graph2 = new Graph(12);
            GraphControl1.SetGraph(graph1);
            GraphControl2.SetGraph(graph2);
            CompareButton.Click += OnCompareButtonClick;
        }

        private void OnCompareButtonClick(object sender, RoutedEventArgs e)
        {
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
                CompareResult.Text = "The graphs are different!";
            }
        }
    }