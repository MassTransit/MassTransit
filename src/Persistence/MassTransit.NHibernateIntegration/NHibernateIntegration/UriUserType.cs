namespace MassTransit.NHibernateIntegration
{
    using System;
    using System.Data.Common;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;


    /// <summary>
    /// An NHibernate user type for storing a Uri
    /// </summary>
    [Serializable]
    public class UriUserType :
        IUserType
    {
        bool IUserType.Equals(object x, object y)
        {
            if (x != null)
                return x.Equals(y);

            if (y != null)
                return y.Equals(x);

            return true;
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var value = (string)NHibernateUtil.String.NullSafeGet(rs, names, session);

            return new Uri(value);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index, session);
                return;
            }

            value = value.ToString();

            NHibernateUtil.String.NullSafeSet(cmd, value, index, session);
        }

        public object DeepCopy(object value)
        {
            return value ?? null;
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

        public SqlType[] SqlTypes => new[] {NHibernateUtil.String.SqlType};

        public Type ReturnedType => typeof(Uri);

        public bool IsMutable => false;
    }
}
