namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class AsyncFactoryActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<BehaviorContext<TSaga>, Task<IStateMachineActivity<TSaga>>> _activityFactory;

        public AsyncFactoryActivity(Func<BehaviorContext<TSaga>, Task<IStateMachineActivity<TSaga>>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("activityFactory");
        }

        async Task IStateMachineActivity<TSaga>.Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            IStateMachineActivity<TSaga> activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Execute(context, next).ConfigureAwait(false);
        }

        async Task IStateMachineActivity<TSaga>.Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
        {
            IStateMachineActivity<TSaga> activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Execute(context, new WidenBehavior<TSaga, T>(next, context)).ConfigureAwait(false);
        }

        async Task IStateMachineActivity<TSaga>.Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
        {
            IStateMachineActivity<TSaga> activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Faulted(context, next).ConfigureAwait(false);
        }

        async Task IStateMachineActivity<TSaga>.Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context,
            IBehavior<TSaga, T> next)
        {
            IStateMachineActivity<TSaga> activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Faulted(context, next).ConfigureAwait(false);
        }
    }


    public class AsyncFactoryActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly Func<BehaviorContext<TSaga, TMessage>, Task<IStateMachineActivity<TSaga, TMessage>>> _activityFactory;

        public AsyncFactoryActivity(Func<BehaviorContext<TSaga, TMessage>, Task<IStateMachineActivity<TSaga, TMessage>>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("activityFactory");
        }

        public async Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            IStateMachineActivity<TSaga, TMessage> activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Execute(context, next).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            IStateMachineActivity<TSaga, TMessage> activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Faulted(context, next).ConfigureAwait(false);
        }
    }
}
