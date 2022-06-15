namespace MassTransit.Visualizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internals;
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
                args.VertexFormat.Shape = args.Vertex.IsComposite ? GraphvizVertexShape.InvHouse : GraphvizVertexShape.Rectangle;

                if (args.Vertex.TargetType != typeof(Event) && args.Vertex.TargetType != typeof(Exception))
                {
                    if (args.Vertex.TargetType.ClosesType(typeof(Fault<>), out Type[] arguments))
                    {
                        args.VertexFormat.Label += "<" + arguments[0].Name + ">";
                    }
                    else
                        args.VertexFormat.Label += "<" + args.Vertex.TargetType.Name + ">";
                }
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

            List<Vertex> compositeTargets = data.Edges.Where(x => x.From.IsComposite).Select(x => x.To).ToList();

            List<Vertex> targets = data.Edges.Select(x => x.To).ToList();
            List<Vertex> vertices = data.Vertices.Where(v => targets.Contains(v) || v.Title == "Initial").ToList();
            List<Edge> edges = data.Edges.Where(x => vertices.Contains(x.From) && !compositeTargets.Contains(x.From)).ToList();

            graph.AddVertexRange(vertices);
            graph.AddEdgeRange(edges.Select(x => new Edge<Vertex>(x.From, x.To)));
            return graph;
        }
    }
}
