namespace MassTransit.Configuration
{
    public interface ITransportConfiguration :
        ISpecification
    {
        ITransportConfigurator Configurator { get; }

        int PrefetchCount { get; }

        int? ConcurrentMessageLimit { get; }

        int GetConcurrentMessageLimit();
    }
}
