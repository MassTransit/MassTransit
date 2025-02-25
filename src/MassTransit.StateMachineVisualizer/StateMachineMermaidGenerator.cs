namespace MassTransit.Visualizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Abstractions;
    using Internals;
    using QuikGraph;
    using SagaStateMachine;


    public class StateMachineMermaidGenerator : StateMachineGenerator
    {
        const string OpenBracket = "«";
        const string CloseBracket = "»";

        public StateMachineMermaidGenerator(StateMachineGraph data)
            : base(data)
        {
        }

        public string CreateMermaidFile()
        {
            StringBuilder output = new();
            List<Vertex> vertices = Graph.Vertices.ToList();

            output.Append("flowchart TB;");

            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                var source = FormatVertex(edge.Source, vertices);
                var target = FormatVertex(edge.Target, vertices);
                var line = $"{Environment.NewLine}    {source} --> {target};";

                output.Append(line);
            }

            return output.ToString();
        }

        static string GetVertexLabel(Vertex vertex, bool includeOptionalType)
        {
            if (includeOptionalType && vertex.TargetType != typeof(Event) && vertex.TargetType != typeof(Exception))
            {
                if (vertex.TargetType.ClosesType(typeof(Fault<>), out Type[] arguments))
                    return $"{vertex.Title}{OpenBracket}{arguments[0].Name}{CloseBracket}";

                return $"{vertex.Title}{OpenBracket}{vertex.TargetType.Name}{CloseBracket}";
            }

            return vertex.Title;
        }

        static string FormatVertex(Vertex vertex, List<Vertex> vertices)
        {
            var index = vertices.IndexOf(vertex);

            if (vertex.VertexType == typeof(Event))
            {
                var vertexLabel = GetVertexLabel(vertex, true);

                if (vertex.IsComposite)
                    return $"{index}[\\\"{vertexLabel}\"/]";

                return $"{index}[\"{vertexLabel}\"]";
            }

            return $"{index}([\"{GetVertexLabel(vertex, false)}\"])";
        }
    }
}
