namespace MassTransit.EventHubIntegration.Activities
{
    using System;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public class FaultedProduceActivity<TSaga, TException, TMessage> :
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
        where TException : Exception
    {
        readonly ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TMessage> _messageFactory;
        readonly ExceptionEventHubNameProvider<TSaga, TException> _nameProvider;

        public FaultedProduceActivity(ExceptionEventHubNameProvider<TSaga, TException> nameProvider,
            ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TMessage> messageFactory)
        {
            _nameProvider = nameProvider;
            _messageFactory = messageFactory;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
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
            await Faulted(context).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        public async Task Faulted<T, TOtherException>(BehaviorExceptionContext<TSaga, T, TOtherException> context,
            IBehavior<TSaga, T> next)
            where T : class
            where TOtherException : Exception
        {
            await Faulted(context).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("produce-faulted");
        }

        Task Faulted(BehaviorContext<TSaga> context)
        {
            if (context is BehaviorExceptionContext<TSaga, TException> exceptionContext)
            {
                return _messageFactory.Use(exceptionContext, async (ctx, s) =>
                {
                    var producer = await ctx.GetProducer(ctx, _nameProvider(ctx)).ConfigureAwait(false);

                    await producer.Produce(s.Message, s.Pipe).ConfigureAwait(false);
                });
            }

            return Task.CompletedTask;
        }
    }


    public class FaultedProduceActivity<TSaga, TData, TException, TMessage> :
        IStateMachineActivity<TSaga, TData>
        where TSaga : class, SagaStateMachineInstance
        where TData : class
        where TMessage : class
        where TException : Exception
    {
        readonly ContextMessageFactory<BehaviorExceptionContext<TSaga, TData, TException>, TMessage> _messageFactory;
        readonly ExceptionEventHubNameProvider<TSaga, TData, TException> _nameProvider;

        public FaultedProduceActivity(ExceptionEventHubNameProvider<TSaga, TData, TException> nameProvider,
            ContextMessageFactory<BehaviorExceptionContext<TSaga, TData, TException>, TMessage> messageFactory)
        {
            _messageFactory = messageFactory;
            _nameProvider = nameProvider;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("produce-faulted");
        }

        public Task Execute(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
        {
            return next.Execute(context);
        }

        public async Task Faulted<T>(BehaviorExceptionContext<TSaga, TData, T> context,
            IBehavior<TSaga, TData> next)
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TData, TException> exceptionContext)
            {
                await _messageFactory.Use(exceptionContext, async (ctx, s) =>
                {
                    var producer = await ctx.GetProducer(ctx, _nameProvider(ctx)).ConfigureAwait(false);

                    await producer.Produce(s.Message, s.Pipe).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}
