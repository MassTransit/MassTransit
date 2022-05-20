namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using SagaStateMachine;
    using Scheduling;


    public static class StateMachineRequestExtensions
    {
        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(this EventActivityBinder<TInstance, TData> binder,
            Request<TInstance, TRequest, TResponse> request, EventMessageFactory<TInstance, TData, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TData, TRequest, TResponse>(request, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(this EventActivityBinder<TInstance, TData> binder,
            Request<TInstance, TRequest, TResponse> request, AsyncEventMessageFactory<TInstance, TData, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TData, TRequest, TResponse>(request, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(this EventActivityBinder<TInstance, TData> binder,
            Request<TInstance, TRequest, TResponse> request, Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TRequest>>> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TData, TRequest, TResponse>(request, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider">A provider for the address used for the request</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(this EventActivityBinder<TInstance, TData> binder,
            Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider,
            EventMessageFactory<TInstance, TData, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TData, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider">A provider for the address used for the request</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(this EventActivityBinder<TInstance, TData> binder,
            Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider,
            AsyncEventMessageFactory<TInstance, TData, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TData, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider">A provider for the address used for the request</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(this EventActivityBinder<TInstance, TData> binder,
            Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider,
            Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TRequest>>> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TData, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request,
            EventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TException, TRequest, TResponse>(request, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request,
            AsyncEventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TException, TRequest, TResponse>(request, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request,
            Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TRequest>>> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TException, TRequest, TResponse>(request, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TException> serviceAddressProvider,
            EventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TException, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TException> serviceAddressProvider,
            AsyncEventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TException, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TException> Request<TInstance, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TException> binder, Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TException> serviceAddressProvider,
            Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TRequest>>> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TException, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request,
            EventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse>(request,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse>(request,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request,
            Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TRequest>>> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse>(request,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider,
            EventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> Request<TInstance, TData, TException, TRequest, TResponse>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder, Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider,
            Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TRequest>>> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TRequest : class
            where TResponse : class
            where TException : Exception
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(this EventActivityBinder<TInstance> binder,
            Request<TInstance, TRequest, TResponse> request, EventMessageFactory<TInstance, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TRequest, TResponse>(request, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(this EventActivityBinder<TInstance> binder,
            Request<TInstance, TRequest, TResponse> request, AsyncEventMessageFactory<TInstance, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TRequest, TResponse>(request, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(this EventActivityBinder<TInstance> binder,
            Request<TInstance, TRequest, TResponse> request, Func<BehaviorContext<TInstance>, Task<SendTuple<TRequest>>> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TRequest, TResponse>(request, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(this EventActivityBinder<TInstance> binder,
            Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider,
            EventMessageFactory<TInstance, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity = new RequestActivity<TInstance, TRequest, TResponse>(request, serviceAddressProvider,
                MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(this EventActivityBinder<TInstance> binder,
            Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider,
            AsyncEventMessageFactory<TInstance, TRequest> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity =
                new RequestActivity<TInstance, TRequest, TResponse>(request, serviceAddressProvider, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Send a request to the configured service endpoint, and setup the state machine to accept the response.
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <typeparam name="TResponse">The response message type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="request">The configured request to use</param>
        /// <param name="serviceAddressProvider"></param>
        /// <param name="messageFactory">The request message factory</param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Request<TInstance, TRequest, TResponse>(this EventActivityBinder<TInstance> binder,
            Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider,
            Func<BehaviorContext<TInstance>, Task<SendTuple<TRequest>>> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
        {
            ScheduleTokenId.UseTokenId<RequestTimeoutExpired<TRequest>>(x => x.RequestId);
            var activity =
                new RequestActivity<TInstance, TRequest, TResponse>(request, serviceAddressProvider, MessageFactory<TRequest>.Create(messageFactory));

            return binder.Add(activity);
        }

        /// <summary>
        /// Cancels the request timeout, and clears the request data from the state instance
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="binder"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> CancelRequestTimeout<TInstance, TData, TRequest, TResponse>(
            this EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
            where TData : class
        {
            var activity = new CancelRequestTimeoutActivity<TInstance, TData, TRequest, TResponse>(request);

            return binder.Add(activity);
        }

        /// <summary>
        /// Clears the requestId on the state
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="binder"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> ClearRequest<TInstance, TData, TRequest, TResponse>(
            this EventActivityBinder<TInstance, TData> binder, Request<TInstance, TRequest, TResponse> request)
            where TInstance : class, SagaStateMachineInstance
            where TRequest : class
            where TResponse : class
            where TData : class
        {
            var activity = new ClearRequestActivity<TInstance, TData, TRequest, TResponse>(request);

            return binder.Add(activity);
        }
    }
}
