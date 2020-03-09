namespace MassTransit.Conductor.Configuration.Definition
{
    using Configurators;
    using Server;


    /// <summary>
    /// Instance-specific address for a service endpoint
    /// </summary>
    public class InstanceEndpointDefinition :
        IEndpointDefinition
    {
        readonly IServiceInstance _instance;

        public InstanceEndpointDefinition(IServiceInstance instance)
        {
            _instance = instance;
        }

        bool IEndpointDefinition.IsTemporary => true;

        int? IEndpointDefinition.PrefetchCount => default;

        int? IEndpointDefinition.ConcurrentMessageLimit => default;

        string IEndpointDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            return ServiceEndpointNameFormatter.Instance.InstanceEndpointName(_instance.InstanceName, formatter);
        }

        void IEndpointDefinition.Configure<T>(T configurator)
        {
        }
    }
}
