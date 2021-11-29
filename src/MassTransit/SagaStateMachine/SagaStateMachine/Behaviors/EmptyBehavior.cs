namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class EmptyBehavior<TSaga> :
        IBehavior<TSaga>
        where TSaga : class, ISaga
    {
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
        }

        public Task Execute(BehaviorContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Execute<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context)
            where T : class
            where TException : Exception
        {
            return Task.CompletedTask;
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context)
            where TException : Exception
        {
            return Task.CompletedTask;
        }
    }


    public class EmptyBehavior<TSaga, TMessage> :
        IBehavior<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
        }

        public Task Execute(BehaviorContext<TSaga, TMessage> context)
        {
            return Task.CompletedTask;
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context)
            where TException : Exception
        {
            return Task.CompletedTask;
        }
    }
}
