namespace MassTransit.KafkaIntegration.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using Configuration.Configurators;
    using Confluent.Kafka;
    using Context;
    using GreenPipes;
    using Registration;
    using Transport;


    public class KafkaSubscriptionDefinition<TKey, TValue> :
        IKafkaSubscriptionDefinition
        where TValue : class
    {
        readonly ConsumerBuilder<TKey, TValue> _consumerBuilder;
        readonly ConsumerConfig _consumerConfig;
        readonly IRegistration _registration;

        public KafkaSubscriptionDefinition(IRegistration registration, string topic, ConsumerBuilder<TKey, TValue> consumerBuilder, ILogContext logContext,
            ConsumerConfig consumerConfig)
        {
            _registration = registration;
            _consumerBuilder = consumerBuilder;
            _consumerConfig = consumerConfig;
            Topic = topic;
            LogContext = logContext;
        }

        public string Topic { get; }
        public bool IsAutoCommitEnabled => _consumerConfig.EnableAutoCommit == true;
        public ILogContext LogContext { get; }

        public IKafkaSubscription Build(IBusInstance busInstance)
        {
            IConsumer<TKey, TValue> consumer = _consumerBuilder.Build();
            IKafkaReceiver<TKey, TValue> receiver = CreateReceiver(busInstance, cfg =>
            {
                cfg.ConfigureConsumers(_registration);
                cfg.ConfigureSagas(_registration);
            });
            return new KafkaSubscription<TKey, TValue>(Topic, consumer, receiver, LogContext, IsAutoCommitEnabled);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Topic))
                yield return this.Failure(nameof(Topic), "should not be empty");

            if (string.IsNullOrEmpty(_consumerConfig.GroupId))
                yield return this.Failure("GroupId", "should not be empty");

            if (string.IsNullOrEmpty(_consumerConfig.BootstrapServers))
                yield return this.Failure("BootstrapServers", "should not be empty. Please use cfg.Host() to configure it");
        }

        IKafkaReceiver<TKey, TValue> CreateReceiver(IBusInstance busInstance, Action<IReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration($"kafka-{Topic}");
            var configuration = new KafkaReceiverConfiguration<TKey, TValue>(busInstance, endpointConfiguration);

            configure?.Invoke(configuration);
            return configuration.Build();
        }
    }
}
