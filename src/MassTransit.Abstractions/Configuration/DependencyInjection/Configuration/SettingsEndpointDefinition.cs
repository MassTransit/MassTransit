namespace MassTransit.Configuration
{
    public abstract class SettingsEndpointDefinition<TSettings> :
        IEndpointDefinition<TSettings>
        where TSettings : class
    {
        readonly IEndpointSettings<IEndpointDefinition<TSettings>> _settings;
        string? _endpointName;

        protected SettingsEndpointDefinition(IEndpointSettings<IEndpointDefinition<TSettings>> settings)
        {
            _settings = settings;
        }

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            string FormatName()
            {
                return string.IsNullOrWhiteSpace(_settings.Name)
                    ? FormatEndpointName(formatter)
                    : _settings.Name!;
            }

            return _endpointName ??= string.IsNullOrWhiteSpace(_settings.InstanceId)
                ? FormatName()
                : formatter.SanitizeName(FormatName() + formatter.Separator + _settings.InstanceId);
        }

        public bool IsTemporary => _settings.IsTemporary;
        public int? PrefetchCount => _settings.PrefetchCount;
        public int? ConcurrentMessageLimit => _settings.ConcurrentMessageLimit;
        public bool ConfigureConsumeTopology => _settings.ConfigureConsumeTopology;

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }

        protected abstract string FormatEndpointName(IEndpointNameFormatter formatter);
    }
}
