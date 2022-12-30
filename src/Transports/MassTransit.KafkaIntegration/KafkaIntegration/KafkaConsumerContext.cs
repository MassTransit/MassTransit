namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using Confluent.Kafka;
    using Logging;
    using MassTransit.Configuration;
    using MassTransit.Middleware;


    public class KafkaConsumerContext :
        BasePipeContext,
        ConsumerContext
    {
        readonly IHostConfiguration _hostConfiguration;
        readonly ConsumerBuilderFactory _consumerBuilderFactory;

        public KafkaConsumerContext(IHostConfiguration hostConfiguration, ConsumerBuilderFactory consumerBuilderFactory, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _hostConfiguration = hostConfiguration;
            _consumerBuilderFactory = consumerBuilderFactory;
        }

        public event Action<Error>? ErrorHandler;

        public ILogContext? LogContext => _hostConfiguration.ReceiveLogContext;

        public IConsumer<byte[], byte[]> CreateConsumer(KafkaConsumerBuilderContext context, Action<IConsumer<byte[], byte[]>, Error> onError)
        {
            return _consumerBuilderFactory.Invoke()
                .SetErrorHandler((c, e) =>
                {
                    ErrorHandler?.Invoke(e);
                    onError.Invoke(c, e);
                })
                .SetPartitionsLostHandler(context.OnPartitionLost)
                .SetPartitionsAssignedHandler(context.OnAssigned)
                .SetPartitionsRevokedHandler(context.OnUnAssigned)
                .Build();
        }
    }
}
