namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Announces that a service endpoint is up and available to accept that specified message type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Up<T>
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

        /// <summary>
        /// The other endpoints known by this endpoint
        /// </summary>
        EndpointInfo[] Peers { get; }
    }
}
