namespace MassTransit
{
    public interface IEndpointSettings<T>
        where T : class
    {
        string? Name { get; }

        bool IsTemporary { get; }

        int? PrefetchCount { get; }

        int? ConcurrentMessageLimit { get; }

        bool ConfigureConsumeTopology { get; }

        string? InstanceId { get; set; }
    }
}
