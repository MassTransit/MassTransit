namespace MassTransit.Visualizer
{
    using System;
    using System.Linq;
    using QuikGraph;
    using QuikGraph.Graphviz;
    using QuikGraph.Graphviz.Dot;
    using SagaStateMachine;


    public class StateMachineGraphvizGenerator
    {
        readonly AdjacencyGraph<Vertex, Edge<Vertex>> _graph;

        public StateMachineGraphvizGenerator(StateMachineGraph data)
        {
            _graph = CreateAdjacencyGraph(data);
        }

        public string CreateDotFile()
        {
            var algorithm = new GraphvizAlgorithm<Vertex, Edge<Vertex>>(_graph);
            algorithm.FormatVertex += VertexStyler;
            return algorithm.Generate();
        }

        static void VertexStyler(object sender, FormatVertexEventArgs<Vertex> args)
        {
            args.VertexFormat.Label = args.Vertex.Title;

            if (args.Vertex.VertexType == typeof(Event))
            {
                args.VertexFormat.FontColor = GraphvizColor.Black;
                args.VertexFormat.Shape = GraphvizVertexShape.Rectangle;

                if (args.Vertex.TargetType != typeof(Event) && args.Vertex.TargetType != typeof(Exception))
                    args.VertexFormat.Label += "<" + args.Vertex.TargetType.Name + ">";
            }
            else
            {
                switch (args.Vertex.Title)
                {
                    case "Initial":
                        args.VertexFormat.FillColor = GraphvizColor.White;
                        break;
                    case "Final":
                        args.VertexFormat.FillColor = GraphvizColor.White;
                        break;
                    default:
                        args.VertexFormat.FillColor = GraphvizColor.White;
                        args.VertexFormat.FontColor = GraphvizColor.Black;
                        break;
                }

                args.VertexFormat.Shape = GraphvizVertexShape.Ellipse;
            }
        }

        static AdjacencyGraph<Vertex, Edge<Vertex>> CreateAdjacencyGraph(StateMachineGraph data)
        {
            var graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();

            graph.AddVertexRange(data.Vertices);
            graph.AddEdgeRange(data.Edges.Select(x => new Edge<Vertex>(x.From, x.To)));
            return graph;
        }
    }
}
