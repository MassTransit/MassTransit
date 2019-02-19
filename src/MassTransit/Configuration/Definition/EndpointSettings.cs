namespace MassTransit.Definition
{
    public class EndpointSettings<T> :
        IEndpointSettings<T>
        where T : class
    {
        public string Name { get; set; }

        public bool IsTemporary { get; set; }

        public int? PrefetchCount { get; set; }

        public int? ConcurrentMessageLimit { get; set; }
    }
}
