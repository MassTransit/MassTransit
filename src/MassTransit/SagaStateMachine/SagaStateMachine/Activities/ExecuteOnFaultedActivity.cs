namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ExecuteOnFaultedActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
    {
        readonly IStateMachineActivity<TSaga> _activity;

        public ExecuteOnFaultedActivity(IStateMachineActivity<TSaga> activity)
        {
            _activity = activity;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            _activity.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _activity.Probe(context);
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

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception
        {
            var nextBehavior = new ExecuteOnFaultedBehavior<TSaga, TException>(next, context);

            return _activity.Execute(context, nextBehavior);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where TException : Exception
            where T : class
        {
            var nextBehavior = new ExecuteOnFaultedBehavior<TSaga, T, TException>(next, context);

            return _activity.Execute(context, nextBehavior);
        }
    }
}
