namespace MassTransit.NHibernateIntegration
{
    using System;
    using System.Data.Common;
    using MassTransit;
    using NHibernate.Engine;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;


    public class StateMachineStateUserType<T> :
        IUserType
        where T : StateMachine
    {
        static StateUserTypeConverter _converter;
        static Func<T> _machine = () => (T)Activator.CreateInstance(typeof(T));

        bool IUserType.Equals(object x, object y)
        {
            var xs = (State)x;
            var ys = (State)y;

            return xs.Name.Equals(ys.Name);
        }

        public int GetHashCode(object x)
        {
            return ((State)x).Name.GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var converter = GetConverter();

            return converter.Get(rs, names, session);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            var converter = GetConverter();

            converter.Set(cmd, value, index, session);
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public SqlType[] SqlTypes
        {
            get
            {
                var converter = GetConverter();

                return converter.Types;
            }
        }

        public Type ReturnedType => typeof(State);

        public bool IsMutable => false;

        public static void SaveAsString(T machine)
        {
            _machine = () => machine;
            _converter = new StringStateUserTypeConverter<T>(machine);
        }

        public static void SaveAsInt32(T machine, params State[] states)
        {
            _machine = () => machine;
            _converter = new IntStateUserTypeConverter<T>(machine, states);
        }

        public static void SetStateUserTypeConverter(StateUserTypeConverter converter)
        {
            _converter = converter;
        }

        public static void SetStateUserTypeConverter(T machine, StateUserTypeConverter converter)
        {
            _machine = () => machine;
            _converter = converter;
        }

        StateUserTypeConverter GetConverter()
        {
            return _converter ?? (_converter = new StringStateUserTypeConverter<T>(_machine()));
        }
    }
}
