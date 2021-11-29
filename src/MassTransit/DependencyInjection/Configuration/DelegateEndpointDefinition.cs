namespace MassTransit.Configuration
{
    public class DelegateEndpointDefinition :
        IEndpointDefinition
    {
        readonly IDefinition _definition;
        readonly IEndpointDefinition _endpointDefinition;
        readonly string _endpointName;

        public DelegateEndpointDefinition(string endpointName, IDefinition definition, IEndpointDefinition endpointDefinition)
        {
            _endpointName = endpointName;
            _definition = definition;
            _endpointDefinition = endpointDefinition;
        }

        public bool ConfigureConsumeTopology => _endpointDefinition?.ConfigureConsumeTopology ?? true;

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _endpointName;
        }

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            _endpointDefinition?.Configure(configurator);
        }

        public bool IsTemporary => _endpointDefinition?.IsTemporary ?? false;

        public int? PrefetchCount => _endpointDefinition?.PrefetchCount;

        public int? ConcurrentMessageLimit => _definition.ConcurrentMessageLimit ?? _endpointDefinition?.ConcurrentMessageLimit;
    }
}
