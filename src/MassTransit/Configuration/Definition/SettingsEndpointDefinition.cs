namespace MassTransit.Definition
{
    public abstract class SettingsEndpointDefinition<TSettings> :
        IEndpointDefinition<TSettings>
        where TSettings : class
    {
        readonly IEndpointSettings<IEndpointDefinition<TSettings>> _settings;
        string _endpointName;

        protected SettingsEndpointDefinition(IEndpointSettings<IEndpointDefinition<TSettings>> settings)
        {
            _settings = settings;
        }

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _endpointName ??= string.IsNullOrWhiteSpace(_settings.Name)
                ? FormatEndpointName(formatter)
                : _settings.Name;
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
