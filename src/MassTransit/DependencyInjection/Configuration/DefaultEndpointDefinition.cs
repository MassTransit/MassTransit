namespace MassTransit.Configuration
{
    /// <summary>
    /// Base/Default endpoint definition, not used apparently
    /// </summary>
    public abstract class DefaultEndpointDefinition :
        IEndpointDefinition
    {
        protected DefaultEndpointDefinition(bool isTemporary = false)
        {
            IsTemporary = isTemporary;
        }

        public virtual bool ConfigureConsumeTopology => true;

        public abstract string GetEndpointName(IEndpointNameFormatter formatter);

        public virtual bool IsTemporary { get; }

        public virtual int? PrefetchCount => default;

        public virtual int? ConcurrentMessageLimit => default;

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }
    }
}
