namespace MassTransit.JobService.Configuration
{
    using System.Text;
    using Components;


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

        public bool ConfigureConsumeTopology => true;

        string IEndpointDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            var sb = new StringBuilder(_instance.InstanceName.Length + 9);

            sb.Append("Instance");
            sb.Append('_');
            sb.Append(_instance.InstanceName);

            return formatter.SanitizeName(sb.ToString());
        }

        void IEndpointDefinition.Configure<T>(T configurator)
        {
        }
    }
}
