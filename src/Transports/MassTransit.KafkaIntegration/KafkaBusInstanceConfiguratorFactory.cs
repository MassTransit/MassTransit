namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Registration;


    public class KafkaBusInstanceConfiguratorFactory<TContainerContext> :
        IBusAttachmentRegistrationFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly ClientConfig _clientConfig;
        readonly Action<IBusAttachmentRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> _configure;

        public KafkaBusInstanceConfiguratorFactory(Action<IBusAttachmentRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
            : this(null, configure)
        {
        }

        public KafkaBusInstanceConfiguratorFactory(ClientConfig clientConfig,
            Action<IBusAttachmentRegistrationContext<TContainerContext>, IKafkaFactoryConfigurator> configure)
        {
            _clientConfig = clientConfig;
            _configure = configure;
        }

        public IBusInstanceConfigurator CreateBusAttachment(IBusAttachmentRegistrationContext<TContainerContext> context)
        {
            var factoryConfigurator = new KafkaFactoryConfigurator(_clientConfig ?? context.GetService<ClientConfig>() ?? new ClientConfig());
            _configure?.Invoke(context, factoryConfigurator);
            return factoryConfigurator.Build();
        }
    }
}
