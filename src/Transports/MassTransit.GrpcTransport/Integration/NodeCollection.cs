namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Contexts;
    using Fabric;
    using GreenPipes.Agents;


    public class NodeCollection :
        INodeCollection
    {
        readonly ConcurrentDictionary<Uri, IGrpcNode> _instances;
        readonly IMessageFabric _messageFabric;
        readonly ISupervisor _supervisor;

        public NodeCollection(ISupervisor supervisor, IMessageFabric messageFabric)
        {
            _supervisor = supervisor;
            _messageFabric = messageFabric;
            _instances = new ConcurrentDictionary<Uri, IGrpcNode>();
        }

        public IEnumerator<IGrpcNode> GetEnumerator()
        {
            return _instances.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IGrpcNode HostNode { get; set; }

        public IGrpcNode GetNode(NodeContext context)
        {
            return _instances.GetOrAdd(context.NodeAddress, _ => CreateNode(context));
        }

        public bool IsHostNode(NodeContext context)
        {
            return HostNode.NodeAddress.Equals(context.NodeAddress);
        }

        IGrpcNode CreateNode(NodeContext context)
        {
            var instance = new GrpcNode(_messageFabric, context);

            _supervisor.Add(instance);

            return instance;
        }
    }
}
