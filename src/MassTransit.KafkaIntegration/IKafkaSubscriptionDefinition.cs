namespace MassTransit.KafkaIntegration
{
    using System;
    using Configuration.Configurators;
    using Confluent.Kafka;
    using Context;
    using Registration;
    using Transport;


    public interface IKafkaSubscriptionDefinition
    {
        string Topic { get; }
        bool IsAutoCommitEnabled { get; }
        ILogContext LogContext { get; }
        IKafkaSubscription Build(IBusInstance busInstance, IRegistration registration);
    }


    public class KafkaSubscriptionDefinition<TKey, TValue> :
        IKafkaSubscriptionDefinition
        where TValue : class
    {
        readonly IConsumer<TKey, TValue> _consumer;
        readonly ConsumerConfig _consumerConfig;

        public KafkaSubscriptionDefinition(string topic, IConsumer<TKey, TValue> consumer, ILogContext logContext, ConsumerConfig consumerConfig)
        {
            _consumer = consumer;
            _consumerConfig = consumerConfig;
            Topic = topic;
            LogContext = logContext;
        }

        public string Topic { get; }
        public bool IsAutoCommitEnabled => _consumerConfig.EnableAutoCommit == true;
        public ILogContext LogContext { get; }

        public IKafkaSubscription Build(IBusInstance busInstance, IRegistration registration)
        {
            IKafkaReceiver<TKey, TValue> receiver = CreateReceiver(busInstance, cfg =>
            {
                cfg.ConfigureConsumers(registration);
                cfg.ConfigureSagas(registration);
            });
            return new KafkaSubscription<TKey, TValue>(Topic, _consumer, receiver, LogContext, IsAutoCommitEnabled);
        }

        IKafkaReceiver<TKey, TValue> CreateReceiver(IBusInstance busInstance, Action<IReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration(Topic);
            var configuration = new KafkaReceiverConfiguration<TKey, TValue>(busInstance, endpointConfiguration);

            configure?.Invoke(configuration);
            return configuration.Build();
        }
    }
}
