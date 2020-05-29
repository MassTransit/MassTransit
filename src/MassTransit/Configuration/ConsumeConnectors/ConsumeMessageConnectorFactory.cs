namespace MassTransit.ConsumeConnectors
{
    using System;
    using Pipeline.Filters;


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
