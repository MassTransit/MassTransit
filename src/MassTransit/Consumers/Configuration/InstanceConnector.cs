namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Util;


    public class InstanceConnector<TConsumer> :
        IInstanceConnector
        where TConsumer : class
    {
        readonly IEnumerable<IInstanceMessageConnector<TConsumer>> _connectors;

        public InstanceConnector()
        {
            if (MessageTypeCache<TConsumer>.HasSagaInterfaces)
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            _connectors = Consumes()
                .ToList();
        }

        public ConnectHandle ConnectInstance<T>(IConsumePipeConnector pipeConnector, T instance, IConsumerSpecification<T> specification)
            where T : class
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (IInstanceMessageConnector<T> connector in _connectors.Cast<IInstanceMessageConnector<T>>())
                {
                    var handle = connector.ConnectInstance(pipeConnector, instance, specification);

                    handles.Add(handle);
                }

                return new MultipleConnectHandle(handles);
            }
            catch (Exception)
            {
                foreach (var handle in handles)
                    handle.Dispose();
                throw;
            }
        }

        public ConnectHandle ConnectInstance(IConsumePipeConnector pipeConnector, object instance)
        {
            if (instance is TConsumer consumer)
            {
                IConsumerSpecification<TConsumer> specification = CreateConsumerSpecification<TConsumer>();

                return ConnectInstance(pipeConnector, consumer, specification);
            }

            throw new ConsumerException(
                $"The instance type {TypeCache.GetShortName(instance.GetType())} does not match the consumer type: {TypeCache<TConsumer>.ShortName}");
        }

        public IConsumerSpecification<T> CreateConsumerSpecification<T>()
            where T : class
        {
            List<IConsumerMessageSpecification<T>> messageSpecifications =
                _connectors.Select(x => x.CreateConsumerMessageSpecification())
                    .Cast<IConsumerMessageSpecification<T>>()
                    .ToList();

            return new ConsumerSpecification<T>(messageSpecifications);
        }

        static IEnumerable<IInstanceMessageConnector<TConsumer>> Consumes()
        {
            return ConsumerMetadataCache<TConsumer>.ConsumerTypes.Select(x => x.GetInstanceConnector<TConsumer>());
        }
    }
}
