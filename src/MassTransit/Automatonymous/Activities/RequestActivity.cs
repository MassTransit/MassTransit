namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;


    public class RequestActivity<TInstance, TRequest, TResponse> :
        RequestActivityImpl<TInstance, TRequest, TResponse>,
        Activity<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        readonly AsyncEventMessageFactory<TInstance, TRequest> _asyncMessageFactory;
        readonly EventMessageFactory<TInstance, TRequest> _messageFactory;
        readonly ServiceAddressProvider<TInstance> _serviceAddressProvider;

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, EventMessageFactory<TInstance, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider,
            EventMessageFactory<TInstance, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, AsyncEventMessageFactory<TInstance, TRequest> messageFactory)
            : base(request)
        {
            _asyncMessageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider,
            AsyncEventMessageFactory<TInstance, TRequest> messageFactory)
            : base(request)
        {
            _asyncMessageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            ConsumeEventContext<TInstance> consumeContext = context.CreateConsumeContext();

            var requestMessage = _messageFactory?.Invoke(consumeContext) ?? await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            await SendRequest(context, consumeContext, requestMessage, _serviceAddressProvider(consumeContext)).ConfigureAwait(false);
        }
    }


    public class RequestActivity<TInstance, TData, TRequest, TResponse> :
        RequestActivityImpl<TInstance, TRequest, TResponse>,
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TRequest : class
        where TResponse : class
    {
        readonly AsyncEventMessageFactory<TInstance, TData, TRequest> _asyncMessageFactory;
        readonly EventMessageFactory<TInstance, TData, TRequest> _messageFactory;
        readonly ServiceAddressProvider<TInstance, TData> _serviceAddressProvider;

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, EventMessageFactory<TInstance, TData, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider,
            EventMessageFactory<TInstance, TData, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, AsyncEventMessageFactory<TInstance, TData, TRequest> messageFactory)
            : base(request)
        {
            _asyncMessageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider,
            AsyncEventMessageFactory<TInstance, TData, TRequest> messageFactory)
            : base(request)
        {
            _asyncMessageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var requestMessage = _messageFactory?.Invoke(consumeContext) ?? await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            await SendRequest(context, consumeContext, requestMessage, _serviceAddressProvider(consumeContext)).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
