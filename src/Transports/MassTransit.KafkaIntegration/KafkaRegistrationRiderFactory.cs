namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Registration;


    public class KafkaRegistrationRiderFactory<TContainerContext> :
        IRegistrationRiderFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly ClientConfig _clientConfig;
        readonly Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> _configure;

        public KafkaRegistrationRiderFactory(Action<IRiderRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            : this(null, configure)
        {
        }

        public KafkaRegistrationRiderFactory(ClientConfig clientConfig,
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
