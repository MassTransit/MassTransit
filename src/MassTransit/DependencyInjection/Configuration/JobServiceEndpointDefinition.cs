namespace MassTransit.Configuration
{
    using System.Text;
    using JobService;
    using NewIdFormatters;


    public class JobServiceEndpointDefinition :
        IEndpointDefinition<JobService>
    {
        readonly InstanceJobServiceSettings _jobServiceSettings;
        readonly IEndpointSettings<IEndpointDefinition<JobService>> _settings;

        public JobServiceEndpointDefinition(IEndpointSettings<IEndpointDefinition<JobService>> settings, InstanceJobServiceSettings jobServiceSettings)
        {
            _settings = settings;
            _jobServiceSettings = jobServiceSettings;

            var instanceId = NewId.Next();

            InstanceName = instanceId.ToString(ZBase32Formatter.LowerCase);
        }

        string InstanceName { get; }

        public bool IsTemporary => true;
        public int? PrefetchCount => _settings.PrefetchCount;
        public int? ConcurrentMessageLimit => _settings.ConcurrentMessageLimit;
        public bool ConfigureConsumeTopology => _settings.ConfigureConsumeTopology;

        public void Configure<T>(T configurator, IRegistrationContext context)
            where T : IReceiveEndpointConfigurator
        {
            _jobServiceSettings.ApplyConfiguration(configurator);

            _settings.ConfigureEndpoint(configurator, context);
        }

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            var sb = new StringBuilder(InstanceName.Length + 9);

            sb.Append("Instance");
            sb.Append('_');
            sb.Append(InstanceName);

            return formatter.SanitizeName(sb.ToString());
        }
    }
}
