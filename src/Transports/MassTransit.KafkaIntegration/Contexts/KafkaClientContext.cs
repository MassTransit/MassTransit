namespace MassTransit.KafkaIntegration.Contexts
{
    using System.Threading;
    using Confluent.Kafka;
    using GreenPipes;


    public class KafkaClientContext :
        BasePipeContext,
        ClientContext
    {
        public KafkaClientContext(ClientConfig config, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            Config = config;
        }

        public ClientConfig Config { get; }
    }
}
