namespace MassTransit.GrpcTransport
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Contracts;
    using Grpc.Core;


    public interface IGrpcNode :
        IAgent,
        NodeContext
    {
        ChannelWriter<TransportMessage> Writer { get; }

        Task Connect(IAsyncStreamWriter<TransportMessage> writer, IAsyncStreamReader<TransportMessage> reader, CancellationToken cancellationToken);

        void Join(NodeContext context, IEnumerable<Contracts.Topology> topologies);
    }
}
