namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Linq;


    public class StateAccessorIndex<TSaga>
        where TSaga : class, ISaga
    {
        readonly State<TSaga>[] _assignedStates;
        readonly StateMachine<TSaga> _stateMachine;
        readonly Lazy<State<TSaga>[]> _states;

        public StateAccessorIndex(StateMachine<TSaga> stateMachine, State<TSaga> initial, State<TSaga> final, State[] states)
        {
            _stateMachine = stateMachine;

            _assignedStates = new[] { null, initial, final }.Concat(states.Cast<State<TSaga>>()).ToArray();

            _states = new Lazy<State<TSaga>[]>(CreateStateArray);
        }

        public int this[string name]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));

                for (var i = 1; i < _states.Value.Length; i++)
                {
                    if (_states.Value[i].Name.Equals(name))
                        return i;
                }

                throw new ArgumentException("Unknown state specified: " + name);
            }
        }

        public State<TSaga> this[int index]
        {
            get
            {
                if (index < 0 || index >= _states.Value.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _states.Value[index];
            }
        }

        State<TSaga>[] CreateStateArray()
        {
            return _assignedStates.Concat(_stateMachine.States.Cast<State<TSaga>>()).Distinct().ToArray();
        }
    }
}
