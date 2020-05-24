namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Registration;
    using Registration.Attachments;


    public class KafkaBusInstanceConfiguratorFactory<TContainerContext> :
        IBusAttachmentRegistrationFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly ClientConfig _clientConfig;
        readonly Action<IRegistration, IKafkaFactoryConfigurator> _configure;

        public KafkaBusInstanceConfiguratorFactory(Action<IRegistration, IKafkaFactoryConfigurator> configure)
            : this(null, configure)
        {
        }

        public KafkaBusInstanceConfiguratorFactory(ClientConfig clientConfig, Action<IRegistration, IKafkaFactoryConfigurator> configure)
        {
            _clientConfig = clientConfig;
            _configure = configure;
        }

        public IBusInstanceConfigurator CreateBusInstanceConfigurator(IRegistrationContext<TContainerContext> context)
        {
            var factoryConfigurator = new KafkaFactoryConfigurator(context, _clientConfig ?? context.GetService<ClientConfig>() ?? new ClientConfig());
            _configure?.Invoke(context, factoryConfigurator);
            return factoryConfigurator.Build();
        }
    }
}
