namespace MassTransit.Configuration
{
    using GreenPipes;


    public interface ITransportConfiguration :
        ISpecification
    {
        ITransportConfigurator Configurator { get; }

        int PrefetchCount { get; }

        int? ConcurrentMessageLimit { get; }

        int GetConcurrentMessageLimit();
    }
}
