namespace MassTransit.KafkaIntegration
{
    using MassTransit.Configuration;
    using Transports;


    public class ConsumerContextSupervisor :
        TransportPipeContextSupervisor<ConsumerContext>,
        IConsumerContextSupervisor
    {
        public ConsumerContextSupervisor(IHostConfiguration hostConfiguration,
            IClientContextSupervisor clientContextSupervisor, ReceiveSettings receiveSettings,
            ConsumerBuilderFactory consumerBuilderFactory)
            : base(new ConsumerContextFactory(hostConfiguration, clientContextSupervisor, receiveSettings, consumerBuilderFactory))
        {
            clientContextSupervisor.AddConsumeAgent(this);
        }
    }
}
