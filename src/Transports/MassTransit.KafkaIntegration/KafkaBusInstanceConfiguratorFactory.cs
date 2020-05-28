namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Registration;


    public class KafkaBusInstanceConfiguratorFactory<TContainerContext> :
        IRiderRegistrationFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly ClientConfig _clientConfig;
        readonly Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> _configure;

        public KafkaBusInstanceConfiguratorFactory(Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            : this(null, configure)
        {
        }

        public KafkaBusInstanceConfiguratorFactory(ClientConfig clientConfig,
            Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
        {
            _clientConfig = clientConfig;
            _configure = configure;
        }

        public IBusInstanceSpecification CreateRider(IRiderRegistrationContext<TContainerContext> context)
        {
            var factoryConfigurator = new KafkaFactoryConfigurator(_clientConfig ?? context.GetService<ClientConfig>() ?? new ClientConfig());
            _configure?.Invoke(context, factoryConfigurator);
            return factoryConfigurator.Build();
        }
    }
}
