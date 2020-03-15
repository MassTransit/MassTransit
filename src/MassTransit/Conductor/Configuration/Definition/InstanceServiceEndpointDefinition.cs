namespace MassTransit.Conductor.Configuration.Definition
{
    using Configurators;
    using Server;


    /// <summary>
    /// Instance-specific address for a service endpoint
    /// </summary>
    public class InstanceServiceEndpointDefinition :
        IEndpointDefinition
    {
        readonly IEndpointDefinition _definition;
        readonly IServiceInstance _instance;

        public InstanceServiceEndpointDefinition(IEndpointDefinition definition, IServiceInstance instance)
        {
            _definition = definition;
            _instance = instance;
        }

        bool IEndpointDefinition.IsTemporary => _definition.IsTemporary;

        int? IEndpointDefinition.PrefetchCount => _definition.PrefetchCount;

        int? IEndpointDefinition.ConcurrentMessageLimit => _definition.ConcurrentMessageLimit;

        public bool ConfigureConsumeTopology => false;

        string IEndpointDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            var endpointName = _definition.GetEndpointName(formatter);

            return ServiceEndpointNameFormatter.Instance.InstanceServiceEndpointName(endpointName, _instance.InstanceName, formatter);
        }

        void IEndpointDefinition.Configure<T>(T configurator)
        {
            _definition.Configure(configurator);
        }
    }
}
