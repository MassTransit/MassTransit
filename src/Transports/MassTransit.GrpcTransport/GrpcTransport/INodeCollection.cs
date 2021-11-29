namespace MassTransit.GrpcTransport
{
    using System.Collections.Generic;


    public interface INodeCollection :
        IEnumerable<IGrpcNode>
    {
        IGrpcNode GetNode(NodeContext context);
    }
}
