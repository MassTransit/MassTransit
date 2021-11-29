namespace MassTransit
{
    public interface ITransportConfigurator
    {
        int PrefetchCount { set; }

        int? ConcurrentMessageLimit { set; }
    }
}
