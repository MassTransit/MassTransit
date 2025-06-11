namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class FaultedContainerFactoryActivity<TSaga, TException, TActivity> :
        IStateMachineActivity<TSaga>
        where TActivity : class, IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TException : Exception
    {
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

        public Task Faulted<TOtherException>(BehaviorExceptionContext<TSaga, TOtherException> context, IBehavior<TSaga> next)
            where TOtherException : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TException> exceptionContext)
            {
                var activity = context.GetServiceOrCreateInstance<TActivity>();

                return activity.Faulted(exceptionContext, next);
            }

            return next.Faulted(context);
        }

        public Task Faulted<T, TOtherException>(BehaviorExceptionContext<TSaga, T, TOtherException> context, IBehavior<TSaga, T> next)
            where T : class
            where TOtherException : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, T, TException> exceptionContext)
            {
                var activity = context.GetServiceOrCreateInstance<TActivity>();

                return activity.Faulted(exceptionContext, next);
            }

            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("containerActivityFactory");
        }
    }


    public class FaultedContainerFactoryActivity<TSaga, TMessage, TException, TActivity> :
        IStateMachineActivity<TSaga, TMessage>
        where TActivity : class, IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TException : Exception
        where TMessage : class
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("containerActivityFactory");
        }

        public Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            return next.Execute(context);
        }

        public Task Faulted<T>(BehaviorExceptionContext<TSaga, TMessage, T> context, IBehavior<TSaga, TMessage> next)
            where T : Exception
        {
            if (context is BehaviorExceptionContext<TSaga, TMessage, TException> exceptionContext)
            {
                var activity = context.GetServiceOrCreateInstance<TActivity>();

                return activity.Faulted(exceptionContext, next);
            }

            return next.Faulted(context);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
