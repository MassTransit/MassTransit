namespace MassTransit.KafkaIntegration
{
    using Confluent.Kafka;
    using Transports;


    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        public ClientContextSupervisor(ClientConfig clientConfig)
            : base(new ClientContextFactory(clientConfig))
        {
        }
    }
}
