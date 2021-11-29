namespace MassTransit.NHibernateIntegration
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.SqlTypes;


    /// <summary>
    /// The default storage
    /// </summary>
    /// <typeparam name="TStateMachine"></typeparam>
    public class StringStateUserTypeConverter<TStateMachine> :
        StateUserTypeConverter
        where TStateMachine : StateMachine
    {
        static readonly SqlType[] _types = { NHibernateUtil.String.SqlType };
        readonly Dictionary<string, State> _stateCache;

        public StringStateUserTypeConverter(TStateMachine machine)
        {
            _stateCache = new Dictionary<string, State>(machine.States.ToDictionary(x => x.Name));
        }

        public SqlType[] Types => _types;

        public void Set(DbCommand command, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(command, null, index, session);
                return;
            }

            value = ((State)value).Name;

            NHibernateUtil.String.NullSafeSet(command, value, index, session);
        }

        public State Get(DbDataReader rs, string[] names, ISessionImplementor session)
        {
            var value = (string)NHibernateUtil.String.NullSafeGet(rs, names, session);

            var state = _stateCache[value];

            return state;
        }
    }
}
