namespace MassTransit.Definition
{
    using System;
    using Saga;


    public class EndpointSagaDefinition<TSaga> :
        ISagaDefinition<TSaga>
        where TSaga : class, ISaga
    {
        readonly IEndpointDefinition<TSaga> _endpointDefinition;

        public EndpointSagaDefinition(IEndpointDefinition<TSaga> endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;
        }

        public void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> consumerConfigurator)
        {
        }

        public Type SagaType => typeof(TSaga);

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _endpointDefinition.GetEndpointName(formatter);
        }

        public int? ConcurrentMessageLimit => _endpointDefinition.ConcurrentMessageLimit;
    }
}
