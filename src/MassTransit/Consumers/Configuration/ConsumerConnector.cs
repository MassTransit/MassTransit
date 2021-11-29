namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Util;


    public class ConsumerConnector<T> :
        IConsumerConnector
        where T : class
    {
        readonly IEnumerable<IConsumerMessageConnector<T>> _connectors;

        public ConsumerConnector()
        {
            if (MessageTypeCache<T>.HasSagaInterfaces)
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            _connectors = Consumes()
                .ToList();
        }

        public IEnumerable<IConsumerMessageConnector> Connectors => _connectors;

        ConnectHandle IConsumerConnector.ConnectConsumer<TConsumer>(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory,
            IConsumerSpecification<TConsumer> specification)
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (IConsumerMessageConnector<TConsumer> connector in _connectors.Cast<IConsumerMessageConnector<TConsumer>>())
                {
                    var handle = connector.ConnectConsumer(consumePipe, consumerFactory, specification);

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

        IConsumerSpecification<TConsumer> IConsumerConnector.CreateConsumerSpecification<TConsumer>()
        {
            List<IConsumerMessageSpecification<TConsumer>> messageSpecifications =
                _connectors.Select(x => x.CreateConsumerMessageSpecification())
                    .Cast<IConsumerMessageSpecification<TConsumer>>()
                    .ToList();

            return new ConsumerSpecification<TConsumer>(messageSpecifications);
        }

        static IEnumerable<IConsumerMessageConnector<T>> Consumes()
        {
            return ConsumerMetadataCache<T>.ConsumerTypes.Select(x => x.GetConsumerConnector<T>());
        }
    }
}
