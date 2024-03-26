namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class CatchBehaviorBuilder<TSaga> :
        IBehaviorBuilder<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        readonly List<IStateMachineActivity<TSaga>> _activities;
        readonly Lazy<IBehavior<TSaga>> _behavior;

        public CatchBehaviorBuilder()
        {
            _activities = new List<IStateMachineActivity<TSaga>>();
            _behavior = new Lazy<IBehavior<TSaga>>(CreateBehavior);
        }

        public IBehavior<TSaga> Behavior => _behavior.Value;

        public void Add(IStateMachineActivity<TSaga> activity)
        {
            if (_behavior.IsValueCreated)
                throw new SagaStateMachineException("The behavior was already built, additional activities cannot be added.");

            _activities.Add(activity);
        }

        IBehavior<TSaga> CreateBehavior()
        {
            if (_activities.Count == 0)
                return SagaStateMachine.Behavior.Empty<TSaga>();

            IBehavior<TSaga> current = new LastCatchBehavior(_activities[_activities.Count - 1]);

            for (var i = _activities.Count - 2; i >= 0; i--)
                current = new ActivityBehavior<TSaga>(_activities[i], current);

            return current;
        }


        class LastCatchBehavior :
            IBehavior<TSaga>
        {
            readonly IStateMachineActivity<TSaga> _activity;

            public LastCatchBehavior(IStateMachineActivity<TSaga> activity)
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
                return _activity.Execute(context, SagaStateMachine.Behavior.Empty<TSaga>());
            }

            public Task Execute<T>(BehaviorContext<TSaga, T> context)
                where T : class
            {
                return _activity.Execute(context, SagaStateMachine.Behavior.Empty<TSaga, T>());
            }

            public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context)
                where T : class
                where TException : Exception
            {
                return _activity.Faulted(context, SagaStateMachine.Behavior.Empty<TSaga, T>());
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context)
                where TException : Exception
            {
                return _activity.Faulted(context, SagaStateMachine.Behavior.Empty<TSaga>());
            }
        }
    }
}
