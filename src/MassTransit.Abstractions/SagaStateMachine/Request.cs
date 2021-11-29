namespace MassTransit
{
    using System;
    using Contracts;


    /// <summary>
    /// A request is a state-machine based request configuration that includes
    /// the events and states related to the execution of a request.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    /// <typeparam name="TSaga"></typeparam>
    public interface Request<in TSaga, TRequest, TResponse>
        where TSaga : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// The name of the request
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The settings that are used for the request, including the timeout
        /// </summary>
        RequestSettings Settings { get; }

        /// <summary>
        /// The event that is raised when the request completes and the response is received
        /// </summary>
        Event<TResponse> Completed { get; set; }

        /// <summary>
        /// The event raised when the request faults
        /// </summary>
        Event<Fault<TRequest>> Faulted { get; set; }

        /// <summary>
        /// The event raised when the request times out with no response received
        /// </summary>
        Event<RequestTimeoutExpired<TRequest>> TimeoutExpired { get; set; }

        /// <summary>
        /// The state that is transitioned to once the request is pending
        /// </summary>
        State Pending { get; set; }

        /// <summary>
        /// Sets the requestId on the instance using the configured property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="requestId"></param>
        void SetRequestId(TSaga instance, Guid? requestId);

        /// <summary>
        /// Gets the requestId on the instance using the configured property
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Guid? GetRequestId(TSaga instance);

        /// <summary>
        /// Generate a requestId, using either the CorrelationId of the saga, or a NewId
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Guid GenerateRequestId(TSaga instance);

        /// <summary>
        /// Set the headers on the outgoing request <see cref="SendContext{TRequest}"/>
        /// </summary>
        /// <param name="context"></param>
        void SetSendContextHeaders(SendContext<TRequest> context);
    }


    /// <summary>
    /// A request is a state-machine based request configuration that includes
    /// the events and states related to the execution of a request.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TResponse2"></typeparam>
    public interface Request<in TSaga, TRequest, TResponse, TResponse2> :
        Request<TSaga, TRequest, TResponse>
        where TSaga : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TResponse2 : class
    {
        /// <summary>
        /// The event that is raised when the request completes and the response is received
        /// </summary>
        Event<TResponse2> Completed2 { get; set; }
    }


    /// <summary>
    /// A request is a state-machine based request configuration that includes
    /// the events and states related to the execution of a request.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TResponse2"></typeparam>
    /// <typeparam name="TResponse3"></typeparam>
    public interface Request<in TSaga, TRequest, TResponse, TResponse2, TResponse3> :
        Request<TSaga, TRequest, TResponse, TResponse2>
        where TSaga : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TResponse2 : class
        where TResponse3 : class
    {
        /// <summary>
        /// The event that is raised when the request completes and the response is received
        /// </summary>
        Event<TResponse3> Completed3 { get; set; }
    }
}
