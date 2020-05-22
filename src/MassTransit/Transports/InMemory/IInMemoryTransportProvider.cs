namespace MassTransit.Transports.InMemory
{
    using Builders;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;


    public interface IInMemoryTransportProvider :
        ISupervisor,
        ISendTransportProvider,
        IPublishTransportProvider,
        IProbeSite
    {
        IReceiveTransport GetReceiveTransport(string queueName, ReceiveEndpointContext receiveEndpointContext);

        IInMemoryConsumeTopologyBuilder CreateConsumeTopologyBuilder();
    }
}
