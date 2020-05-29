namespace MassTransit
{
    using Transports;


    public interface IReceiveEndpointControl :
        IReceiveEndpoint
    {
        /// <summary>
        /// Starts receiving from the inbound transport.
        /// </summary>
        /// <returns>A handle to the receiving endpoint, which is used to stop it</returns>
        ReceiveEndpointHandle Start();
    }
}
