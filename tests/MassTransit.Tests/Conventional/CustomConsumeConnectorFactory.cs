namespace MassTransit.Tests.Conventional
{
    using System;
    using MassTransit.Configuration;


    public class CustomConsumeConnectorFactory<TConsumer, TMessage> :
        IMessageConnectorFactory
        where TConsumer : class, IHandler<TMessage>
        where TMessage : class
    {
        readonly ConsumerMessageConnector<TConsumer, TMessage> _consumerConnector;
        readonly InstanceMessageConnector<TConsumer, TMessage> _instanceConnector;

        public CustomConsumeConnectorFactory()
        {
            var filter = new CustomMethodConsumerMessageFilter<TConsumer, TMessage>();

            _consumerConnector = new ConsumerMessageConnector<TConsumer, TMessage>(filter);
            _instanceConnector = new InstanceMessageConnector<TConsumer, TMessage>(filter);
        }

        IConsumerMessageConnector<T> IMessageConnectorFactory.CreateConsumerConnector<T>()
        {
            return _consumerConnector as IConsumerMessageConnector<T> ?? throw new ArgumentException("The consumer type did not match the connector type");
        }

        IInstanceMessageConnector<T> IMessageConnectorFactory.CreateInstanceConnector<T>()
        {
            return _instanceConnector as IInstanceMessageConnector<T> ?? throw new ArgumentException("The consumer type did not match the connector type");
        }
    }
}
