namespace MassTransit
{
    using System;
    using Contracts;


    /// <summary>
    /// The request settings include the address of the request handler, as well as the timeout to use
    /// for requests.
    /// </summary>
    public interface RequestSettings<TSaga, TRequest, TResponse>
        where TSaga : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// The endpoint address of the service that handles the request
        /// </summary>
        Uri ServiceAddress { get; }

        /// <summary>
        /// The timeout period before the request times out
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// If true, the requestId is cleared when Faulted is triggered
        /// </summary>
        bool ClearRequestIdOnFaulted { get; }

        /// <summary>
        /// If specified, the TimeToLive is set on the outgoing request
        /// </summary>
        TimeSpan? TimeToLive { get; }

        /// <summary>
        /// Configures the behavior of the Completed event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TSaga, TResponse>> Completed { get; }

        /// <summary>
        /// Configures the behavior of the Faulted event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TSaga, Fault<TRequest>>> Faulted { get; }

        /// <summary>
        /// Configures the behavior of the Timeout Expired event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TSaga, RequestTimeoutExpired<TRequest>>> TimeoutExpired { get; }
    }


    /// <summary>
    /// The request settings include the address of the request handler, as well as the timeout to use
    /// for requests.
    /// </summary>
    public interface RequestSettings<TSaga, TRequest, TResponse, TResponse2> :
        RequestSettings<TSaga, TRequest, TResponse>
        where TSaga : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TResponse2 : class
    {
        /// <summary>
        /// Configures the behavior of the Completed event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TSaga, TResponse2>> Completed2 { get; }
    }


    /// <summary>
    /// The request settings include the address of the request handler, as well as the timeout to use
    /// for requests.
    /// </summary>
    public interface RequestSettings<TSaga, TRequest, TResponse, TResponse2, TResponse3> :
        RequestSettings<TSaga, TRequest, TResponse, TResponse2>
        where TSaga : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TResponse2 : class
        where TResponse3 : class
    {
        /// <summary>
        /// Configures the behavior of the Completed event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TSaga, TResponse3>> Completed3 { get; }
    }
}
