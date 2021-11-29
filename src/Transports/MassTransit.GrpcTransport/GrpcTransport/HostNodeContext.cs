namespace MassTransit.GrpcTransport
{
    using System;
    using Metadata;


    public class HostNodeContext :
        NodeContext
    {
        public HostNodeContext(Uri nodeAddress)
        {
            NodeAddress = nodeAddress;
            SessionId = NewId.NextGuid();
        }

        public NodeType NodeType => NodeType.Host;

        public Uri NodeAddress { get; }

        public Guid SessionId { get; }

        public HostInfo Host
        {
            get => HostMetadataCache.Host;
            set => throw new InvalidOperationException("The Host cannot be set on the Host");
        }
    }
}
