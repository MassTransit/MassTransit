namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// The Send Endpoint Provider is used to retrieve endpoints using addresses. The interface is
    /// available both at the bus and within the context of most message receive handlers, including
    /// the consume context, saga context, consumer context, etc. The most local provider should be
    /// used to ensure message continuity is maintained.
    /// </summary>
    public interface ISendEndpointProvider :
        ISendObserverConnector
    {
        /// <summary>
        /// Return the send endpoint for the specified address
        /// </summary>
        /// <param name="address">The endpoint address</param>
        /// <returns>The send endpoint</returns>
        Task<ISendEndpoint> GetSendEndpoint(Uri address);
    }
}
