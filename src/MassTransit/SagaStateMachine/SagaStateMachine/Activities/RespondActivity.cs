namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class RespondActivity<TSaga, TMessage, T> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
        where T : class
    {
        readonly ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> _messageFactory;

        public RespondActivity(ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("respond");
        }

        public async Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            await _messageFactory.Use(context, (ctx, s) => ctx.RespondAsync(s.Message, s.Pipe)).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
