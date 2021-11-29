namespace MassTransit.Configuration
{
    public class EndpointRegistrationConfigurator<T> :
        IEndpointRegistrationConfigurator
        where T : class
    {
        readonly EndpointSettings<IEndpointDefinition<T>> _settings;

        public EndpointRegistrationConfigurator()
        {
            _settings = new EndpointSettings<IEndpointDefinition<T>>();
        }

        public IEndpointSettings<IEndpointDefinition<T>> Settings => _settings;

        public string Name
        {
            set => _settings.Name = value;
        }

        public bool Temporary
        {
            set => _settings.IsTemporary = value;
        }

        public int? PrefetchCount
        {
            set => _settings.PrefetchCount = value;
        }

        public int? ConcurrentMessageLimit
        {
            set => _settings.ConcurrentMessageLimit = value;
        }

        public bool ConfigureConsumeTopology
        {
            set => _settings.ConfigureConsumeTopology = value;
        }

        public string InstanceId
        {
            set => _settings.InstanceId = value;
        }
    }
}
