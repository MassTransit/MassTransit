namespace MassTransit.Configuration
{
    using System;
    using Middleware;


    public class ConsumeMessageConnectorFactory<TConsumer, TMessage> :
        IMessageConnectorFactory
        where TConsumer : class, IConsumer<TMessage>
        where TMessage : class
    {
        readonly ConsumerMessageConnector<TConsumer, TMessage> _consumerConnector;
        readonly InstanceMessageConnector<TConsumer, TMessage> _instanceConnector;

        public ConsumeMessageConnectorFactory()
        {
            var filter = new MethodConsumerMessageFilter<TConsumer, TMessage>();

            _consumerConnector = new ConsumerMessageConnector<TConsumer, TMessage>(filter);
            _instanceConnector = new InstanceMessageConnector<TConsumer, TMessage>(filter);
        }

        public IConsumerMessageConnector<T> CreateConsumerConnector<T>()
            where T : class
        {
            return _consumerConnector as IConsumerMessageConnector<T> ?? throw new ArgumentException("The consumer type did not match the connector type");
        }

        IInstanceMessageConnector<T> IMessageConnectorFactory.CreateInstanceConnector<T>()
        {
            return _instanceConnector as IInstanceMessageConnector<T> ?? throw new ArgumentException("The consumer type did not match the connector type");
        }
    }
}
