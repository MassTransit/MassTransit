namespace MassTransit
{
    public interface IInMemoryReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Sets the concurrency message limit, as should be received
        /// </summary>
        int ConcurrencyLimit { set; }
    }
}
