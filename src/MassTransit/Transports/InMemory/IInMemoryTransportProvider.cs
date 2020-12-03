namespace MassTransit.Transports.InMemory
{
    using Builders;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Agents;


    public interface IInMemoryTransportProvider :
        IAgent,
        ISendTransportProvider,
        IPublishTransportProvider,
        IProbeSite
    {
        IMessageFabric MessageFabric { get; }

        IInMemoryConsumeTopologyBuilder CreateConsumeTopologyBuilder();
    }
}
