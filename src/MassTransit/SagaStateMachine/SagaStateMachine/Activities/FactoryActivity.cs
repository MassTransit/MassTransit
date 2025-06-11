namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class FactoryActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        readonly Func<BehaviorContext<TSaga>, IStateMachineActivity<TSaga>> _activityFactory;

        public FactoryActivity(Func<BehaviorContext<TSaga>, IStateMachineActivity<TSaga>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("factory");
        }

        public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            IStateMachineActivity<TSaga> activity = _activityFactory(context);

            return activity.Execute(context, next);
        }

        public Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class
        {
            IStateMachineActivity<TSaga> activity = _activityFactory(context);

            return activity.Execute(context, next);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception
        {
            IStateMachineActivity<TSaga> activity = _activityFactory(context);

            return activity.Faulted(context, next);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where T : class
            where TException : Exception
        {
            IStateMachineActivity<TSaga> activity = _activityFactory(context);

            return activity.Faulted(context, next);
        }
    }


    public class FactoryActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly Func<BehaviorContext<TSaga, TMessage>, IStateMachineActivity<TSaga, TMessage>> _activityFactory;

        public FactoryActivity(Func<BehaviorContext<TSaga, TMessage>, IStateMachineActivity<TSaga, TMessage>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("factory");
        }

        public Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            IStateMachineActivity<TSaga, TMessage> activity = _activityFactory(context);

            return activity.Execute(context, next);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            IStateMachineActivity<TSaga, TMessage> activity = _activityFactory(context);

            return activity.Faulted(context, next);
        }
    }
}
