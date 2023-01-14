namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Transports;


    public class ProducerContextSupervisor :
        TransportPipeContextSupervisor<ProducerContext>,
        IProducerContextSupervisor
    {
        readonly Action _onStop;

        public ProducerContextSupervisor(IClientContextSupervisor clientContextSupervisor, IHostConfiguration hostConfiguration,
            Func<ProducerBuilder<byte[], byte[]>> producerBuilderFactory, Action onStop)
            : base(new ProducerContextFactory(clientContextSupervisor, hostConfiguration, producerBuilderFactory))
        {
            _onStop = onStop;
            clientContextSupervisor.AddSendAgent(this);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            await base.StopSupervisor(context);
            _onStop?.Invoke();
        }
    }
}
