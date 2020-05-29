namespace MassTransit.Tests.Conventional
{
    using System;
    using ConsumeConnectors;


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
            var result = _consumerConnector as IConsumerMessageConnector<T>;
            if (result == null)
                throw new ArgumentException("The consumer type did not match the connector type");

            return result;
        }

        IInstanceMessageConnector<T> IMessageConnectorFactory.CreateInstanceConnector<T>()
        {
            var result = _instanceConnector as IInstanceMessageConnector<T>;
            if (result == null)
                throw new ArgumentException("The consumer type did not match the connector type");

            return result;
        }
    }
}
