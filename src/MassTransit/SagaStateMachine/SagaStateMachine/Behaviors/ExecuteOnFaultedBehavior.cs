namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ExecuteOnFaultedBehavior<TSaga, TException> :
        IBehavior<TSaga>
        where TException : Exception
        where TSaga : class, ISaga
    {
        readonly BehaviorExceptionContext<TSaga, TException> _context;
        readonly IBehavior<TSaga> _next;

        public ExecuteOnFaultedBehavior(IBehavior<TSaga> next, BehaviorExceptionContext<TSaga, TException> context)
        {
            _next = next;
            _context = context;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            _next.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _next.Probe(context);
        }

        Task IBehavior<TSaga>.Execute(BehaviorContext<TSaga> context)
        {
            return _next.Faulted(_context);
        }

        Task IBehavior<TSaga>.Execute<T>(BehaviorContext<TSaga, T> context)
        {
            return _next.Faulted(_context);
        }

        Task IBehavior<TSaga>.Faulted<TData, T>(BehaviorExceptionContext<TSaga, TData, T> context)
        {
            throw new SagaStateMachineException("This should not ever be called.");
        }

        Task IBehavior<TSaga>.Faulted<T>(BehaviorExceptionContext<TSaga, T> context)
        {
            throw new SagaStateMachineException("This should not ever be called.");
        }
    }


    public class ExecuteOnFaultedBehavior<TSaga, TMessage, TException> :
        IBehavior<TSaga>
        where TException : Exception
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly BehaviorExceptionContext<TSaga, TMessage, TException> _context;
        readonly IBehavior<TSaga, TMessage> _next;

        public ExecuteOnFaultedBehavior(IBehavior<TSaga, TMessage> next, BehaviorExceptionContext<TSaga, TMessage, TException> context)
        {
            _next = next;
            _context = context;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            _next.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _next.Probe(context);
        }

        Task IBehavior<TSaga>.Execute(BehaviorContext<TSaga> context)
        {
            return _next.Faulted(_context);
        }

        Task IBehavior<TSaga>.Execute<T>(BehaviorContext<TSaga, T> context)
        {
            return _next.Faulted(_context);
        }

        Task IBehavior<TSaga>.Faulted<TD, T>(BehaviorExceptionContext<TSaga, TD, T> context)
        {
            throw new SagaStateMachineException("This should not ever be called.");
        }

        Task IBehavior<TSaga>.Faulted<T>(BehaviorExceptionContext<TSaga, T> context)
        {
            throw new SagaStateMachineException("This should not ever be called.");
        }
    }
}
