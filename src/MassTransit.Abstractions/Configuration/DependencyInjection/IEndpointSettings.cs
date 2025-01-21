namespace MassTransit
{
    public interface IEndpointSettings<TConsumer>
        where TConsumer : class
    {
        string? Name { get; }

        bool IsTemporary { get; }

        int? PrefetchCount { get; }

        int? ConcurrentMessageLimit { get; }

        bool ConfigureConsumeTopology { get; }

        string? InstanceId { get; }

        void ConfigureEndpoint<T>(T configurator, IRegistrationContext? context)
            where T : IReceiveEndpointConfigurator;
    }
}
