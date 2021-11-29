namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ContainerFactoryActivity<TSaga, TActivity> :
        IStateMachineActivity<TSaga>
        where TActivity : class, IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
    {
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            var factory = context.GetStateMachineActivityFactory();

            var activity = factory.GetService<TActivity>(context);

            return activity.Execute(context, next);
        }

        public Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            var factory = context.GetStateMachineActivityFactory();

            var activity = factory.GetService<TActivity>(context);

            var widenBehavior = new WidenBehavior<TSaga, T>(next, context);

            return activity.Execute(context, widenBehavior);
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

        public void Probe(ProbeContext context)
        {
            context.CreateScope("containerActivityFactory");
        }
    }


    public class ContainerFactoryActivity<TSaga, TMessage, TActivity> :
        IStateMachineActivity<TSaga, TMessage>
        where TActivity : class, IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateScope("containerActivityFactory");
        }

        public Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            var factory = context.GetStateMachineActivityFactory();

            var activity = factory.GetService<TActivity>(context);

            return activity.Execute(context, next);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
