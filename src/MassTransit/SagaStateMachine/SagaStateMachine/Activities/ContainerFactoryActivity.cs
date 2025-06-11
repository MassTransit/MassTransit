namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ContainerFactoryActivity<TSaga, TActivity> :
        IStateMachineActivity<TSaga>
        where TActivity : class, IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            var activity = context.GetServiceOrCreateInstance<TActivity>();

            return activity.Execute(context, next);
        }

        public Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            var activity = context.GetServiceOrCreateInstance<TActivity>();

            return activity.Execute(context, next);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception
        {
            var activity = context.GetServiceOrCreateInstance<TActivity>();

            return activity.Faulted(context, next);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where T : class
            where TException : Exception
        {
            var activity = context.GetServiceOrCreateInstance<TActivity>();

            return activity.Faulted(context, next);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("containerActivityFactory");
        }
    }


    public class ContainerFactoryActivity<TSaga, TMessage, TActivity> :
        IStateMachineActivity<TSaga, TMessage>
        where TActivity : class, IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateScope("containerActivityFactory");
        }

        public Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            var activity = context.GetServiceOrCreateInstance<TActivity>();

            return activity.Execute(context, next);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            var activity = context.GetServiceOrCreateInstance<TActivity>();

            return activity.Faulted(context, next);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
