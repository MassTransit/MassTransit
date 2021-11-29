namespace MassTransit.KafkaIntegration.Configuration
{
    using System;
    using Confluent.Kafka;
    using DependencyInjection;
    using MassTransit.Configuration;
    using Microsoft.Extensions.DependencyInjection;


    public class KafkaRegistrationRiderFactory :
        IRegistrationRiderFactory<IKafkaRider>
    {
        readonly ClientConfig _clientConfig;
        readonly Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> _configure;

        public KafkaRegistrationRiderFactory(Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configure)
            : this(null, configure)
        {
        }

        public KafkaRegistrationRiderFactory(ClientConfig clientConfig, Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configure)
        {
            _clientConfig = clientConfig;
            _configure = configure;
        }

        public IBusInstanceSpecification CreateRider(IRiderRegistrationContext context)
        {
            var configurator = new KafkaFactoryConfigurator(_clientConfig ?? context.GetService<ClientConfig>() ?? new ClientConfig());

            _configure?.Invoke(context, configurator);

            foreach (var registration in context.GetRegistrations<IKafkaProducerRegistration>())
                registration.Register(configurator, context);

            return configurator.Build(context);
        }
    }
}
