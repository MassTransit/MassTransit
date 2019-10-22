namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Announces that a service endpoint is down and no longer available to accept that specified message type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Down<T>
        where T : class
    {
        /// <summary>
        /// The address where consumers, etc. are actually hosted
        /// </summary>
        Uri ServiceAddress { get; }

        /// <summary>
        /// The endpoint info of the service endpoint
        /// </summary>
        EndpointInfo Endpoint { get; }
    }
}