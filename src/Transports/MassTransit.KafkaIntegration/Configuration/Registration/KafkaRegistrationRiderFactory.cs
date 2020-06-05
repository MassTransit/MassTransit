namespace MassTransit.KafkaIntegration.Registration
{
    using System;
    using Configurators;
    using Confluent.Kafka;
    using MassTransit.Registration;
    using Transport;


    public class KafkaRegistrationRiderFactory<TContainerContext> :
        RegistrationRiderFactory<TContainerContext, IKafkaRider>
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

        public override IBusInstanceSpecification CreateRider(IRiderRegistrationContext<TContainerContext> context)
        {
            var configurator = new KafkaFactoryConfigurator(_clientConfig ?? context.GetService<ClientConfig>() ?? new ClientConfig());

            ConfigureRider(configurator, context);

            _configure?.Invoke(context, configurator);

            return configurator.Build();
        }
    }
}
