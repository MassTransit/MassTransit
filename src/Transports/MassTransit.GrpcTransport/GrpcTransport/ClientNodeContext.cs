namespace MassTransit.GrpcTransport
{
    using System;


    public class ClientNodeContext :
        NodeContext
    {
        public ClientNodeContext(Uri nodeAddress)
        {
            NodeAddress = nodeAddress;
            SessionId = NewId.NextGuid();
        }

        public NodeType NodeType => NodeType.Client;
        public Uri NodeAddress { get; }
        public Guid SessionId { get; }
        public HostInfo Host { get; set; }
    }
}
