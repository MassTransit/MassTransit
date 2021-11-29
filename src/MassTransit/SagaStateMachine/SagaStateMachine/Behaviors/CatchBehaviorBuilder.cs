namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;


    public class CatchBehaviorBuilder<TSaga> :
        IBehaviorBuilder<TSaga>
        where TSaga : class, ISaga
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

            IBehavior<TSaga> current = new LastCatchBehavior<TSaga>(_activities[_activities.Count - 1]);

            for (var i = _activities.Count - 2; i >= 0; i--)
                current = new ActivityBehavior<TSaga>(_activities[i], current);

            return current;
        }
    }
}
