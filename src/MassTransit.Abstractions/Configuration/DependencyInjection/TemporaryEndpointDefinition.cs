namespace MassTransit
{
    /// <summary>
    /// Specifies a temporary endpoint, with the prefix "response"
    /// </summary>
    public class TemporaryEndpointDefinition :
        IEndpointDefinition
    {
        readonly string _tag;
        string? _name;

        public TemporaryEndpointDefinition(string? tag = default, int? concurrentMessageLimit = default, int? prefetchCount = default,
            bool configureConsumeTopology = true)
        {
            ConcurrentMessageLimit = concurrentMessageLimit;
            PrefetchCount = prefetchCount;
            ConfigureConsumeTopology = configureConsumeTopology;

            _tag = tag ?? "endpoint";
        }

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _name ??= formatter.TemporaryEndpoint(_tag);
        }

        public bool IsTemporary => true;
        public int? PrefetchCount { get; }
        public int? ConcurrentMessageLimit { get; }
        public bool ConfigureConsumeTopology { get; }

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }
    }
}
