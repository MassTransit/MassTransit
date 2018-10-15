namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Sent to check if a request is able to be processed by the endpoint
    /// </summary>
    public interface Ask<out T>
        where T : class
    {
        /// <summary>
        /// The client asking
        /// </summary>
        Guid ClientId { get; }

        /// <summary>
        /// The service endpoint address, which is typically a queue endpoint
        /// </summary>
        Uri ServiceAddress { get; }

        /// <summary>
        /// Any additional resources required, which are known in advance
        /// </summary>
        Uri[] Resources { get; }

        /// <summary>
        /// If present, the quota associated with the request
        /// </summary>
        Quota Quota { get; }

        /// <summary>
        /// The actual request, if included, which can be accepted, acknowledged, or declined
        /// </summary>
        T Request { get; }
    }


    public interface Accept<T>
        where T : class
    {
        /// <summary>
        /// The number of requests which are accepted
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The endpoint that accepted the request
        /// </summary>
        EndpointInfo Endpoint { get; }
    }


    /// <summary>
    /// When an endpoint declines to handle the request, indicates the reason why the request was declined by the endpoint.
    /// </summary>
    /// <typeparam name="T">The request type</typeparam>
    public interface Reject<T>
        where T : class
    {
        /// <summary>
        /// The request type
        /// </summary>
        Uri RequestType { get; }

        /// <summary>
        /// Numeric status code for the declined request
        /// </summary>
        int? StatusCode { get; }

        /// <summary>
        /// The reason the request was declined
        /// </summary>
        string Reason { get; }
    }
}