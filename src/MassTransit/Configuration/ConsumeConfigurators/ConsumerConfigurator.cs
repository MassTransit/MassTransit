namespace MassTransit.ConsumeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConnectors;
    using ConsumerSpecifications;
    using GreenPipes;


    public class ConsumerConfigurator<TConsumer> :
        IConsumerConfigurator<TConsumer>,
        IReceiveEndpointSpecification
        where TConsumer : class, IConsumer
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly IConsumerSpecification<TConsumer> _specification;

        public ConsumerConfigurator(IConsumerFactory<TConsumer> consumerFactory, IConsumerConfigurationObserver observer)
        {
            _consumerFactory = consumerFactory;

            _specification = ConsumerConnectorCache<TConsumer>.Connector.CreateConsumerSpecification<TConsumer>();

            _specification.ConnectConsumerConfigurationObserver(observer);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
        {
            _specification.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _specification.ConnectConsumerConfigurationObserver(observer);
        }

        public void Message<T>(Action<IConsumerMessageConfigurator<T>> configure)
            where T : class
        {
            _specification.Message(configure);
        }

        public void ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TConsumer, T>> configure)
            where T : class
        {
            _specification.ConsumerMessage(configure);
        }

        public T Options<T>(Action<T> configure = null)
            where T : IOptions, new()
        {
            return _specification.Options(configure);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _consumerFactory.Validate().Concat(_specification.Validate());
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            ConsumerConnectorCache<TConsumer>.Connector.ConnectConsumer(builder, _consumerFactory, _specification);
        }
    }
}
