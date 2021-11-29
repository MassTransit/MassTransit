namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ActionActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
    {
        readonly Action<BehaviorContext<TSaga>> _action;

        public ActionActivity(Action<BehaviorContext<TSaga>> action)
        {
            _action = action;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then");
        }

        public async Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            _action(context);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<TData>(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
            where TData : class
        {
            _action(context);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context,
            IBehavior<TSaga, T> next)
            where T : class
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }


    public class ActionActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly Action<BehaviorContext<TSaga, TMessage>> _action;

        public ActionActivity(Action<BehaviorContext<TSaga, TMessage>> action)
        {
            _action = action;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then");
        }

        public async Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            _action(context);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
