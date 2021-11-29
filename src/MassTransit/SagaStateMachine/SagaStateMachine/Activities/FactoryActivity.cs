namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class FactoryActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
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

        Task IStateMachineActivity<TSaga>.Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            IStateMachineActivity<TSaga> activity = _activityFactory(context);

            return activity.Execute(context, next);
        }

        Task IStateMachineActivity<TSaga>.Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
        {
            IStateMachineActivity<TSaga> activity = _activityFactory(context);

            return activity.Execute(context, new WidenBehavior<TSaga, T>(next, context));
        }

        Task IStateMachineActivity<TSaga>.Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
        {
            IStateMachineActivity<TSaga> activity = _activityFactory(context);

            return activity.Faulted(context, next);
        }

        Task IStateMachineActivity<TSaga>.Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context,
            IBehavior<TSaga, T> next)
        {
            IStateMachineActivity<TSaga> activity = _activityFactory(context);

            return activity.Faulted(context, new WidenBehavior<TSaga, T>(next, context));
        }
    }


    public class FactoryActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, ISaga
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

        Task IStateMachineActivity<TSaga, TMessage>.Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            IStateMachineActivity<TSaga, TMessage> activity = _activityFactory(context);

            return activity.Execute(context, next);
        }

        Task IStateMachineActivity<TSaga, TMessage>.Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context,
            IBehavior<TSaga, TMessage> next)
        {
            IStateMachineActivity<TSaga, TMessage> activity = _activityFactory(context);

            return activity.Faulted(context, next);
        }
    }
}
