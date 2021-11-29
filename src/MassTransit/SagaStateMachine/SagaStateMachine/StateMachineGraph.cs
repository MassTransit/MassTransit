namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    [Serializable]
    public class StateMachineGraph
    {
        readonly Edge[] _edges;
        readonly Vertex[] _vertices;

        public StateMachineGraph(IEnumerable<Vertex> vertices, IEnumerable<Edge> edges)
        {
            _vertices = vertices.ToArray();
            _edges = edges.ToArray();
        }

        public IEnumerable<Vertex> Vertices => _vertices;

        public IEnumerable<Edge> Edges => _edges;
    }
}
