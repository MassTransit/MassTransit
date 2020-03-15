namespace MassTransit.Registration
{
    using Definition;


    public abstract class EndpointRegistrationConfigurator<T>
        where T : class
    {
        readonly EndpointSettings<IEndpointDefinition<T>> _settings;

        protected EndpointRegistrationConfigurator()
        {
            _settings = new EndpointSettings<IEndpointDefinition<T>>();
        }

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

        public IEndpointSettings<IEndpointDefinition<T>> Settings => _settings;
    }
}
