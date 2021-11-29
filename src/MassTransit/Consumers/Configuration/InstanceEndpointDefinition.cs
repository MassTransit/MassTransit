namespace MassTransit.Configuration
{
    using System.Text;
    using NewIdFormatters;


    /// <summary>
    /// Instance-specific address for a service endpoint
    /// </summary>
    public class InstanceEndpointDefinition :
        IEndpointDefinition
    {
        public InstanceEndpointDefinition()
        {
            var instanceId = NewId.Next();

            InstanceName = instanceId.ToString(ZBase32Formatter.LowerCase);
        }

        string InstanceName { get; }

        public bool IsTemporary => true;

        public int? PrefetchCount => default;

        public int? ConcurrentMessageLimit => default;

        public bool ConfigureConsumeTopology => true;

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            var sb = new StringBuilder(InstanceName.Length + 9);

            sb.Append("Instance");
            sb.Append('_');
            sb.Append(InstanceName);

            return formatter.SanitizeName(sb.ToString());
        }

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }
    }
}
