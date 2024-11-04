namespace MassTransit.Visualizer.Abstractions
{
    using System.Collections.Generic;
    using System.Linq;
    using QuikGraph;
    using SagaStateMachine;


    public abstract class StateMachineGenerator
    {
        protected readonly AdjacencyGraph<Vertex, Edge<Vertex>> Graph;

        public StateMachineGenerator(StateMachineGraph data)
        {
            Graph = CreateAdjacencyGraph(data);
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
