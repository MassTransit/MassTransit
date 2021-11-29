namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// The last behavior either completes the last activity in the behavior or
    /// throws the exception if a compensation is in progress
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class LastBehavior<TSaga> :
        IBehavior<TSaga>
        where TSaga : class, ISaga
    {
        readonly IStateMachineActivity<TSaga> _activity;

        public LastBehavior(IStateMachineActivity<TSaga> activity)
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

        public Task Execute(BehaviorContext<TSaga> context)
        {
            return _activity.Execute(context, Behavior.Empty<TSaga>());
        }

        public Task Execute<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            return _activity.Execute(context, Behavior.Empty<TSaga, T>());
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context)
            where T : class
            where TException : Exception
        {
            return _activity.Faulted(context, Behavior.Faulted<TSaga, T>());
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context)
            where TException : Exception
        {
            return _activity.Faulted(context, Behavior.Faulted<TSaga>());
        }
    }
}
