namespace MassTransit.Conductor.Configuration.Definition
{
    using Configurators;


    /// <summary>
    /// Instance-specific address for a service endpoint
    /// </summary>
    public class ServiceControlEndpointDefinition :
        IEndpointDefinition
    {
        readonly IEndpointDefinition _definition;

        public ServiceControlEndpointDefinition(IEndpointDefinition definition)
        {
            _definition = definition;
        }

        bool IEndpointDefinition.IsTemporary => true;

        int? IEndpointDefinition.PrefetchCount => default;

        int? IEndpointDefinition.ConcurrentMessageLimit => default;

        public bool ConfigureConsumeTopology => true;

        string IEndpointDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            var endpointName = _definition.GetEndpointName(formatter);

            return ServiceEndpointNameFormatter.Instance.ServiceControlEndpointName(endpointName, formatter);
        }

        void IEndpointDefinition.Configure<T>(T configurator)
        {
            _definition.Configure(configurator);
        }
    }
}
