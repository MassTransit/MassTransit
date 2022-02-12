namespace MassTransit.Configuration
{
    public class EndpointSettings<T> :
        IEndpointSettings<T>
        where T : class
    {
        public EndpointSettings()
        {
            ConfigureConsumeTopology = true;
        }

        public string? Name { get; set; }

        public bool IsTemporary { get; set; }

        public int? PrefetchCount { get; set; }

        public int? ConcurrentMessageLimit { get; set; }

        public bool ConfigureConsumeTopology { get; set; }

        public string? InstanceId { get; set; }
    }
}
