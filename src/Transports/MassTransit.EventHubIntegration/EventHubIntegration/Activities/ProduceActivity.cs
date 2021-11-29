namespace MassTransit.EventHubIntegration.Activities
{
    using System;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public class ProduceActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly ContextMessageFactory<BehaviorContext<TSaga>, TMessage> _messageFactory;
        readonly EventHubNameProvider<TSaga> _nameProvider;

        public ProduceActivity(EventHubNameProvider<TSaga> nameProvider, ContextMessageFactory<BehaviorContext<TSaga>, TMessage> messageFactory)
        {
            _nameProvider = nameProvider;
            _messageFactory = messageFactory;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("produce");
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

        Task Execute(BehaviorContext<TSaga> context)
        {
            return _messageFactory.Use(context, async (ctx, s) =>
            {
                var producer = await ctx.GetProducer(ctx, _nameProvider(ctx)).ConfigureAwait(false);

                await producer.Produce(s.Message, s.Pipe, ctx.CancellationToken).ConfigureAwait(false);
            });
        }
    }


    public class ProduceActivity<TSaga, TMessage, T> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
        where T : class
    {
        readonly ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> _messageFactory;
        readonly EventHubNameProvider<TSaga, TMessage> _nameProvider;

        public ProduceActivity(EventHubNameProvider<TSaga, TMessage> nameProvider, ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> messageFactory)
        {
            _nameProvider = nameProvider;
            _messageFactory = messageFactory;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("produce");
        }

        public async Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            await _messageFactory.Use(context, async (ctx, s) =>
            {
                var producer = await ctx.GetProducer(ctx, _nameProvider(ctx)).ConfigureAwait(false);

                await producer.Produce(s.Message, s.Pipe, ctx.CancellationToken).ConfigureAwait(false);
            }).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context,
            IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
