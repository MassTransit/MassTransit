namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class FaultedRespondActivity<TSaga, TException, TMessage> :
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TException : Exception
        where TMessage : class
    {
        readonly ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TMessage> _messageFactory;

        public FaultedRespondActivity(ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TMessage> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("respond-faulted");
        }

        Task IStateMachineActivity<TSaga>.Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            return next.Execute(context);
        }

        Task IStateMachineActivity<TSaga>.Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
        {
            return next.Execute(context);
        }

        async Task IStateMachineActivity<TSaga>.Faulted<T>(BehaviorExceptionContext<TSaga, T> context,
            IBehavior<TSaga> next)
        {
            if (context is BehaviorExceptionContext<TSaga, TException> exceptionContext)
                await _messageFactory.Use(exceptionContext, (ctx, s) => ctx.RespondAsync(s.Message, s.Pipe)).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task IStateMachineActivity<TSaga>.Faulted<T, TOtherException>(BehaviorExceptionContext<TSaga, T, TOtherException> context,
            IBehavior<TSaga, T> next)
        {
            if (context is BehaviorExceptionContext<TSaga, TException> exceptionContext)
                await _messageFactory.Use(exceptionContext, (ctx, s) => ctx.RespondAsync(s.Message, s.Pipe)).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }
    }


    public class FaultedRespondActivity<TSaga, TData, TException, TMessage> :
        IStateMachineActivity<TSaga, TData>
        where TSaga : class, SagaStateMachineInstance
        where TData : class
        where TMessage : class
        where TException : Exception
    {
        readonly ContextMessageFactory<BehaviorExceptionContext<TSaga, TData, TException>, TMessage> _messageFactory;

        public FaultedRespondActivity(ContextMessageFactory<BehaviorExceptionContext<TSaga, TData, TException>, TMessage> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("respond-faulted");
        }

        Task IStateMachineActivity<TSaga, TData>.Execute(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
        {
            return next.Execute(context);
        }

        async Task IStateMachineActivity<TSaga, TData>.Faulted<T>(BehaviorExceptionContext<TSaga, TData, T> context,
            IBehavior<TSaga, TData> next)
        {
            if (context is BehaviorExceptionContext<TSaga, TData, TException> exceptionContext)
                await _messageFactory.Use(exceptionContext, (ctx, s) => ctx.RespondAsync(s.Message, s.Pipe)).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}
