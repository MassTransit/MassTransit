namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class FaultedActionActivity<TSaga, TException> :
        IStateMachineActivity<TSaga>
        where TException : Exception
        where TSaga : class, ISaga
    {
        readonly Action<BehaviorExceptionContext<TSaga, TException>> _action;

        public FaultedActionActivity(Action<BehaviorExceptionContext<TSaga, TException>> action)
        {
            _action = action;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then-faulted");
        }

        public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            return next.Execute(context);
        }

        public Task Execute<TData>(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
            where TData : class
        {
            return next.Execute(context);
        }

        public Task Faulted<T>(BehaviorExceptionContext<TSaga, T> context, IBehavior<TSaga> next)
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TException> exceptionContext)
                _action(exceptionContext);

            return next.Faulted(context);
        }

        public Task Faulted<TData, T>(BehaviorExceptionContext<TSaga, TData, T> context, IBehavior<TSaga, TData> next)
            where TData : class
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TData, TException> exceptionContext)
                _action(exceptionContext);

            return next.Faulted(context);
        }
    }


    public class FaultedActionActivity<TSaga, TMessage, TException> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, ISaga
        where TException : Exception
        where TMessage : class
    {
        readonly Action<BehaviorExceptionContext<TSaga, TMessage, TException>> _action;

        public FaultedActionActivity(Action<BehaviorExceptionContext<TSaga, TMessage, TException>> action)
        {
            _action = action;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then-faulted");
        }

        public Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            return next.Execute(context);
        }

        public Task Faulted<T>(BehaviorExceptionContext<TSaga, TMessage, T> context, IBehavior<TSaga, TMessage> next)
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TMessage, TException> exceptionContext)
                _action(exceptionContext);

            return next.Faulted(context);
        }
    }
}
