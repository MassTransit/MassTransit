namespace MassTransit.KafkaIntegration
{
    using System.Threading;
    using Confluent.Kafka;
    using MassTransit.Middleware;


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
