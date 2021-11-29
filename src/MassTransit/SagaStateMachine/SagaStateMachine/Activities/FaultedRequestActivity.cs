namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class FaultedRequestActivity<TSaga, TException, TRequest, TResponse> :
        RequestActivityImpl<TSaga, TRequest, TResponse>,
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TException : Exception
        where TRequest : class
        where TResponse : class
    {
        readonly ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TRequest> _messageFactory;
        readonly ServiceAddressExceptionProvider<TSaga, TException> _serviceAddressProvider;

        public FaultedRequestActivity(Request<TSaga, TRequest, TResponse> request,
            ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public FaultedRequestActivity(Request<TSaga, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TSaga, TException> serviceAddressProvider,
            ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            return next.Execute(context);
        }

        public Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            return next.Execute(context);
        }

        public async Task Faulted<T>(BehaviorExceptionContext<TSaga, T> context, IBehavior<TSaga> next)
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TException> exceptionContext)
            {
                var serviceAddress = _serviceAddressProvider(exceptionContext);

                await _messageFactory.Use(exceptionContext, (ctx, m) => SendRequest(ctx, m, serviceAddress)).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task IStateMachineActivity<TSaga>.Faulted<T, TOtherException>(BehaviorExceptionContext<TSaga, T, TOtherException> context,
            IBehavior<TSaga, T> next)
        {
            if (context is BehaviorExceptionContext<TSaga, TException> exceptionContext)
            {
                var serviceAddress = _serviceAddressProvider(exceptionContext);

                await _messageFactory.Use(exceptionContext, (ctx, m) => SendRequest(ctx, m, serviceAddress)).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }


    public class FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse> :
        RequestActivityImpl<TInstance, TRequest, TResponse>,
        IStateMachineActivity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TException : Exception
        where TRequest : class
        where TResponse : class
    {
        readonly ContextMessageFactory<BehaviorExceptionContext<TInstance, TData, TException>, TRequest> _messageFactory;
        readonly ServiceAddressExceptionProvider<TInstance, TData, TException> _serviceAddressProvider;

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            ContextMessageFactory<BehaviorExceptionContext<TInstance, TData, TException>, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider,
            ContextMessageFactory<BehaviorExceptionContext<TInstance, TData, TException>, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Task Execute(BehaviorContext<TInstance, TData> context, IBehavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        public async Task Faulted<T>(BehaviorExceptionContext<TInstance, TData, T> context, IBehavior<TInstance, TData> next)
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TInstance, TData, TException> exceptionContext)
            {
                var serviceAddress = _serviceAddressProvider(exceptionContext);

                await _messageFactory.Use(exceptionContext, (ctx, m) => SendRequest(ctx, m, serviceAddress)).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}
