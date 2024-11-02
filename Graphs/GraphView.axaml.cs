using Avalonia.Controls;
using Avalonia.Media;
using System;
using Avalonia;
using Avalonia.Controls.Shapes;

namespace Graphs
{
    public partial class GraphView : UserControl
    {
        private readonly Graph _graph;

        public GraphView()
        {
            InitializeComponent();
            _graph = new Graph(2); // Crée un graphe par défaut avec 2 nœuds pour éviter l'erreur
            DrawGraph();
        }

        public GraphView(Graph graph)
        {
            if (graph.NodeCount < 2 || graph.NodeCount > 52)
                throw new ArgumentException("Graph must have between 2 and 52 nodes.");

            InitializeComponent();
            _graph = graph;
            DrawGraph();
        }

        private void DrawGraph()
        {
            // Calcul de position des nœuds sur un cercle
            int nodeCount = _graph.NodeCount;
            double centerX = 80 + nodeCount * 12;
            double centerY = centerX;
            double radius = Math.Min(centerX, centerY) - 50;
            var nodePositions = new (double X, double Y)[nodeCount];

            for (int i = 0; i < nodeCount; i++)
            {
                double angle = 2 * Math.PI * i / nodeCount;
                nodePositions[i] = (centerX + radius * Math.Cos(angle), centerY + radius * Math.Sin(angle));
            }

            // Dessiner les arêtes
            for (int j = 0; j < nodeCount; j++)
            {
                for (int i = 0; i < j; i++)
                {
                    var line = new Line
                    {
                        StartPoint = new Point(nodePositions[i].X, nodePositions[i].Y),
                        EndPoint = new Point(nodePositions[j].X, nodePositions[j].Y),
                        Stroke = _graph.HasEdge(i, j) ? Brushes.Blue : Brushes.Gray,
                        StrokeThickness = 2
                    };
                    GraphCanvas1.Children.Add(line);
                }
            }

            // Dessiner les nœuds
            for (int i = 0; i < nodeCount; i++)
            {
                var circle = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = Brushes.Blue,
                    [Canvas.LeftProperty] = nodePositions[i].X - 10,
                    [Canvas.TopProperty] = nodePositions[i].Y - 10
                };
                GraphCanvas1.Children.Add(circle);
            }
        }
    }
}