namespace MassTransit.GrpcTransport.Integration
{
    using System.Collections.Generic;
    using Contexts;


    public interface INodeCollection :
        IEnumerable<IGrpcNode>
    {
        IGrpcNode HostNode { get; }

        IGrpcNode GetNode(NodeContext context);

        bool IsHostNode(NodeContext context);
    }
}
