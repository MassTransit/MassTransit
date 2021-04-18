namespace MassTransit.GrpcTransport
{
    public interface IGrpcReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Sets the concurrency message limit, as should be received
        /// </summary>
        int ConcurrencyLimit { set; }
    }
}