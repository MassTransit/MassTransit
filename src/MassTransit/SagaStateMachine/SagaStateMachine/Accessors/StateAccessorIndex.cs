namespace MassTransit
{
    using System;
    using System.Linq;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        class StateAccessorIndex
        {
            readonly State<TInstance>[] _assignedStates;
            readonly StateMachine<TInstance> _stateMachine;
            readonly Lazy<State<TInstance>[]> _states;

            public StateAccessorIndex(StateMachine<TInstance> stateMachine, State<TInstance> initial, State<TInstance> final, State[] states)
            {
                _stateMachine = stateMachine;

                _assignedStates = new[] { null, initial, final }.Concat(states.Cast<State<TInstance>>()).ToArray();

                _states = new Lazy<State<TInstance>[]>(CreateStateArray);
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

            public State<TInstance> this[int index]
            {
                get
                {
                    if (index < 0 || index >= _states.Value.Length)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    return _states.Value[index];
                }
            }

            State<TInstance>[] CreateStateArray()
            {
                return _assignedStates.Concat(_stateMachine.States.Cast<State<TInstance>>()).Distinct().ToArray();
            }
        }
    }
}
