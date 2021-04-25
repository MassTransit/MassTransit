namespace MassTransit.GrpcTransport.Integration
{
    using System.Collections.Generic;
    using Contexts;


    public interface INodeCollection :
        IEnumerable<IGrpcNode>
    {
        IGrpcNode GetNode(NodeContext context);
    }
}
