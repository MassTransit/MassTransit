namespace MassTransit.GrpcTransport
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Fabric;
    using Transports.Fabric;


    public class NodeCollection :
        INodeCollection
    {
        readonly IMessageFabric<NodeContext, GrpcTransportMessage> _messageFabric;
        readonly ConcurrentDictionary<Uri, IGrpcNode> _nodes;
        readonly ISupervisor _supervisor;

        public NodeCollection(ISupervisor supervisor, IMessageFabric<NodeContext, GrpcTransportMessage> messageFabric)
        {
            _supervisor = supervisor;
            _messageFabric = messageFabric;
            _nodes = new ConcurrentDictionary<Uri, IGrpcNode>();
        }

        public IEnumerator<IGrpcNode> GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IGrpcNode GetNode(NodeContext context)
        {
            return _nodes.GetOrAdd(context.NodeAddress, _ => CreateNode(context));
        }

        IGrpcNode CreateNode(NodeContext context)
        {
            var instance = new GrpcNode(_messageFabric, context);

            _supervisor.Add(instance);

            return instance;
        }
    }
}
