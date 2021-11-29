namespace MassTransit.GrpcTransport
{
    using System;


    public interface NodeContext
    {
        NodeType NodeType { get; }

        Uri NodeAddress { get; }

        Guid SessionId { get; }

        HostInfo Host { get; set; }
    }
}
