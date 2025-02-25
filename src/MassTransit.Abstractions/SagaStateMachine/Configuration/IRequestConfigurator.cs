namespace MassTransit
{
    using System;
    using Contracts;


    public interface IRequestConfigurator
    {
        /// <summary>
        /// Sets the service address of the request handler
        /// </summary>
        Uri ServiceAddress { set; }

        /// <summary>
        /// Sets the request timeout
        /// </summary>
        TimeSpan Timeout { set; }

        /// <summary>
        /// Set the time to live of the request message sent by the saga. If not specified, and the timeout
        /// is > TimeSpan.Zero, the <see cref="Timeout" /> value is used.
        /// </summary>
        TimeSpan? TimeToLive { set; }

        /// <summary>
        /// By default, the RequestId is not cleared when the request is Faulted. Set to true to clear the requestId
        /// </summary>
        bool ClearRequestIdOnFaulted { set; }
    }


    public interface IRequestConfigurator<TInstance, TRequest, TResponse> :
        IRequestConfigurator
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Configure the behavior of the Completed event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TInstance, TResponse>> Completed { set; }

        /// <summary>
        /// Configure the behavior of the Faulted event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TInstance, Fault<TRequest>>> Faulted { set; }

        /// <summary>
        /// Configure the behavior of the Faulted event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TInstance, RequestTimeoutExpired<TRequest>>> TimeoutExpired { set; }
    }


    public interface IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2> :
        IRequestConfigurator<TInstance, TRequest, TResponse>
        where TInstance : class, SagaStateMachineInstance
        where TResponse : class
        where TResponse2 : class
        where TRequest : class
    {
        /// <summary>
        /// Configure the behavior of the Completed event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TInstance, TResponse2>> Completed2 { set; }
    }


    public interface IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2, TResponse3> :
        IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2>
        where TInstance : class, SagaStateMachineInstance
        where TResponse : class
        where TResponse2 : class
        where TResponse3 : class
        where TRequest : class
    {
        /// <summary>
        /// Configure the behavior of the Completed event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        Action<IEventCorrelationConfigurator<TInstance, TResponse3>> Completed3 { set; }
    }
}
