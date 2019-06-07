namespace MassTransit.Definition
{
    using System;
    using ConsumeConfigurators;


    public class EndpointConsumerDefinition<TConsumer> :
        IConsumerDefinition<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly IEndpointDefinition<TConsumer> _endpointDefinition;

        public EndpointConsumerDefinition(IEndpointDefinition<TConsumer> endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;
        }

        void IConsumerDefinition<TConsumer>.Configure(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator)
        {
        }

        Type IConsumerDefinition.ConsumerType => typeof(TConsumer);

        string IConsumerDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _endpointDefinition.GetEndpointName(formatter);
        }

        public int? ConcurrentMessageLimit => _endpointDefinition.ConcurrentMessageLimit;
    }
}
