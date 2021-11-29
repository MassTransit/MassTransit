namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class RequestActivity<TInstance, TRequest, TResponse> :
        RequestActivityImpl<TInstance, TRequest, TResponse>,
        IStateMachineActivity<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        readonly ContextMessageFactory<BehaviorContext<TInstance>, TRequest> _messageFactory;
        readonly ServiceAddressProvider<TInstance> _serviceAddressProvider;

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, ContextMessageFactory<BehaviorContext<TInstance>, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance> serviceAddressProvider,
            ContextMessageFactory<BehaviorContext<TInstance>, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TInstance> context, IBehavior<TInstance> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<T>(BehaviorContext<TInstance, T> context, IBehavior<TInstance, T> next)
            where T : class
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, IBehavior<TInstance> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, IBehavior<TInstance, T> next)
            where T : class
            where TException : Exception
        {
            return next.Faulted(context);
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            var serviceAddress = _serviceAddressProvider(context);

            await _messageFactory.Use(context, (ctx, m) => SendRequest(ctx, m, serviceAddress)).ConfigureAwait(false);
        }
    }


    public class RequestActivity<TInstance, TData, TRequest, TResponse> :
        RequestActivityImpl<TInstance, TRequest, TResponse>,
        IStateMachineActivity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TRequest : class
        where TResponse : class
    {
        readonly ContextMessageFactory<BehaviorContext<TInstance, TData>, TRequest> _messageFactory;
        readonly ServiceAddressProvider<TInstance, TData> _serviceAddressProvider;

        public RequestActivity(Request<TInstance, TRequest, TResponse> request,
            ContextMessageFactory<BehaviorContext<TInstance, TData>, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, ServiceAddressProvider<TInstance, TData> serviceAddressProvider,
            ContextMessageFactory<BehaviorContext<TInstance, TData>, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TInstance, TData> context, IBehavior<TInstance, TData> next)
        {
            var serviceAddress = _serviceAddressProvider(context);

            await _messageFactory.Use(context, (ctx, m) => SendRequest(ctx, m, serviceAddress)).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, IBehavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
