namespace MassTransit.Configuration
{
    using System;
    using Contracts;


    public class StateMachineRequestConfigurator<TInstance, TRequest, TResponse> :
        IRequestConfigurator<TInstance, TRequest, TResponse>,
        RequestSettings<TInstance, TRequest, TResponse>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        public StateMachineRequestConfigurator()
        {
            Timeout = TimeSpan.FromSeconds(30);
        }

        public RequestSettings<TInstance, TRequest, TResponse> Settings
        {
            get
            {
                if (ServiceAddress == null && EndpointConvention.TryGetDestinationAddress<TRequest>(out var serviceAddress))
                    ServiceAddress = serviceAddress;

                return this;
            }
        }

        public Uri ServiceAddress { get; set; }
        public TimeSpan Timeout { get; set; }
        public bool ClearRequestIdOnFaulted { get; set; }
        public TimeSpan? TimeToLive { get; set; }

        public Action<IEventCorrelationConfigurator<TInstance, TResponse>> Completed { get; set; }
        public Action<IEventCorrelationConfigurator<TInstance, Fault<TRequest>>> Faulted { get; set; }
        public Action<IEventCorrelationConfigurator<TInstance, RequestTimeoutExpired<TRequest>>> TimeoutExpired { get; set; }
    }


    public class StateMachineRequestConfigurator<TInstance, TRequest, TResponse, TResponse2> :
        StateMachineRequestConfigurator<TInstance, TRequest, TResponse>,
        IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2>,
        RequestSettings<TInstance, TRequest, TResponse, TResponse2>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TResponse2 : class
    {
        public StateMachineRequestConfigurator()
        {
            Timeout = TimeSpan.FromSeconds(30);
        }

        public new RequestSettings<TInstance, TRequest, TResponse, TResponse2> Settings
        {
            get
            {
                if (ServiceAddress == null && EndpointConvention.TryGetDestinationAddress<TRequest>(out var serviceAddress))
                    ServiceAddress = serviceAddress;

                return this;
            }
        }

        public Action<IEventCorrelationConfigurator<TInstance, TResponse2>> Completed2 { get; set; }
    }


    public class StateMachineRequestConfigurator<TInstance, TRequest, TResponse, TResponse2, TResponse3> :
        StateMachineRequestConfigurator<TInstance, TRequest, TResponse, TResponse2>,
        IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2, TResponse3>,
        RequestSettings<TInstance, TRequest, TResponse, TResponse2, TResponse3>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TResponse2 : class
        where TResponse3 : class
    {
        public StateMachineRequestConfigurator()
        {
            Timeout = TimeSpan.FromSeconds(30);
        }

        public new RequestSettings<TInstance, TRequest, TResponse, TResponse2, TResponse3> Settings
        {
            get
            {
                if (ServiceAddress == null && EndpointConvention.TryGetDestinationAddress<TRequest>(out var serviceAddress))
                    ServiceAddress = serviceAddress;

                return this;
            }
        }

        public Action<IEventCorrelationConfigurator<TInstance, TResponse3>> Completed3 { get; set; }
    }
}
