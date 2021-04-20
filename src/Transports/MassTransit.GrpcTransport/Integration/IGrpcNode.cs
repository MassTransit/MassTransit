namespace MassTransit.GrpcTransport.Integration
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Grpc.Core;


    public interface IGrpcNode :
        IAgent,
        NodeContext
    {
        ChannelWriter<TransportMessage> Writer { get; }

        Task Connect(IAsyncStreamWriter<TransportMessage> writer, IAsyncStreamReader<TransportMessage> reader, CancellationToken cancellationToken);

        ConnectHandle AddTopology(Topology topology, ConnectHandle handle = default);

        IEnumerable<Topology> GetTopology();

        void Join(NodeContext context, IEnumerable<Topology> topologies);
    }
}
