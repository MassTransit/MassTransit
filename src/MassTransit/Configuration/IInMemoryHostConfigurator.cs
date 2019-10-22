namespace MassTransit
{
    public interface IInMemoryHostConfigurator :
        IHostConfigurator
    {
        /// <summary>
        /// Sets the maximum number of threads used by an in-memory transport, for partitioning
        /// the input queue. This setting also specifies how many threads will be used for dispatching
        /// messages to consumers.
        /// </summary>
        int TransportConcurrencyLimit { set; }

        /// <summary>
        /// Configure the SendTransport cache settings
        /// </summary>
        ICacheConfigurator SendTransportCache { get; }
    }
}
