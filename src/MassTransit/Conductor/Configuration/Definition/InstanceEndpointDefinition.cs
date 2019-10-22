namespace MassTransit.Conductor.Configuration.Definition
{
    using Configurators;
    using Server;


    public class InstanceEndpointDefinition :
        IEndpointDefinition
    {
        readonly IEndpointDefinition _definition;
        readonly IServiceInstance _instance;

        public InstanceEndpointDefinition(IEndpointDefinition definition, IServiceInstance instance)
        {
            _definition = definition;
            _instance = instance;
        }

        bool IEndpointDefinition.IsTemporary => _definition.IsTemporary;

        int? IEndpointDefinition.PrefetchCount => _definition.PrefetchCount;

        int? IEndpointDefinition.ConcurrentMessageLimit => _definition.ConcurrentMessageLimit;

        string IEndpointDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            return ServiceEndpointNameFormatter.Instance.EndpointName(_instance.InstanceId, _definition.GetEndpointName(formatter));
        }

        void IEndpointDefinition.Configure<T>(T configurator)
        {
            _definition.Configure(configurator);
        }
    }
}