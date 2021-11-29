namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class SendActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly DestinationAddressProvider<TSaga> _destinationAddressProvider;
        readonly ContextMessageFactory<BehaviorContext<TSaga>, TMessage> _messageFactory;

        public SendActivity(DestinationAddressProvider<TSaga> destinationAddressProvider,
            ContextMessageFactory<BehaviorContext<TSaga>, TMessage> messageFactory)
        {
            _destinationAddressProvider = destinationAddressProvider;
            _messageFactory = messageFactory;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("send");
        }

        public async Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where T : class
            where TException : Exception
        {
            return next.Faulted(context);
        }

        async Task Execute(BehaviorContext<TSaga> context)
        {
            var destinationAddress = _destinationAddressProvider(context);

            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await _messageFactory.Use(context, (ctx, s) => endpoint.Send(s.Message, s.Pipe)).ConfigureAwait(false);
        }
    }


    public class SendActivity<TSaga, TData, TMessage> :
        IStateMachineActivity<TSaga, TData>
        where TSaga : class, SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly DestinationAddressProvider<TSaga, TData> _destinationAddressProvider;
        readonly ContextMessageFactory<BehaviorContext<TSaga, TData>, TMessage> _messageFactory;

        public SendActivity(DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            ContextMessageFactory<BehaviorContext<TSaga, TData>, TMessage> messageFactory)
        {
            _destinationAddressProvider = destinationAddressProvider;
            _messageFactory = messageFactory;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("send");
        }

        public async Task Execute(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
        {
            var destinationAddress = _destinationAddressProvider(context);

            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await _messageFactory.Use(context, (ctx, s) => endpoint.Send(s.Message, s.Pipe)).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TData, TException> context, IBehavior<TSaga, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
