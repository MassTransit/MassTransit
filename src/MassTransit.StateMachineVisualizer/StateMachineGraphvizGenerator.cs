namespace MassTransit.Visualizer
{
    using System;
    using Abstractions;
    using Internals;
    using QuikGraph;
    using QuikGraph.Graphviz;
    using QuikGraph.Graphviz.Dot;
    using SagaStateMachine;


    public class StateMachineGraphvizGenerator : StateMachineGenerator
    {
        public StateMachineGraphvizGenerator(StateMachineGraph data)
            : base(data)
        {
        }

        public string CreateDotFile()
        {
            var algorithm = new GraphvizAlgorithm<Vertex, Edge<Vertex>>(Graph);
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
                        args.VertexFormat.Label += "<" + arguments[0].Name + ">";
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
    }
}
