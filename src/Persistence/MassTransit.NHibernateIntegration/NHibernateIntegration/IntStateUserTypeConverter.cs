namespace MassTransit.NHibernateIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.SqlTypes;


    /// <summary>
    /// Converts a State to an int, based on the given ordered array of states, for storage
    /// using NHibernate. If a new state is added, it must be added to the *end* of the array
    /// to avoid renumbering previously persisted state machine instances.
    /// </summary>
    /// <typeparam name="TStateMachine">The state machine type</typeparam>
    public class IntStateUserTypeConverter<TStateMachine> :
        StateUserTypeConverter
        where TStateMachine : StateMachine
    {
        static readonly SqlType[] _types = { NHibernateUtil.Int32.SqlType };

        readonly Dictionary<State, int> _stateToValueCache;
        readonly Dictionary<int, State> _valueToStateCache;

        public IntStateUserTypeConverter(TStateMachine machine, params State[] states)
        {
            if (machine.States.Except(states).Any())
                throw new ArgumentOutOfRangeException(nameof(states), "One or more states are not specified");

            List<KeyValuePair<int, State>> allStates =
                states.Select((state, index) => new KeyValuePair<int, State>(index, state)).ToList();

            _valueToStateCache = new Dictionary<int, State>(allStates.ToDictionary(x => x.Key, x => x.Value));
            _stateToValueCache = new Dictionary<State, int>(allStates.ToDictionary(x => x.Value, x => x.Key));
        }

        public SqlType[] Types => _types;

        public State Get(DbDataReader rs, string[] names, ISessionImplementor session)
        {
            var value = (int)NHibernateUtil.Int32.NullSafeGet(rs, names, session);

            var state = _valueToStateCache[value];

            return state;
        }

        public void Set(DbCommand command, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.Int32.NullSafeSet(command, null, index, session);
                return;
            }

            var setValue = _stateToValueCache[(State)value];

            NHibernateUtil.Int32.NullSafeSet(command, setValue, index, session);
        }
    }
}
