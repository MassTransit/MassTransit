namespace MassTransit.NHibernateIntegration
{
    using System;
    using System.Data;
    using System.Data.Common;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;


    // taken from https://gist.github.com/3075912
    public class BinaryGuidType :
        IUserType
    {
        static readonly int[] _byteOrder = {10, 11, 12, 13, 14, 15, 8, 9, 6, 7, 4, 5, 0, 1, 2, 3};
        static readonly SqlType[] _types = {new SqlType(DbType.Binary)};

        public SqlType[] SqlTypes => _types;

        public Type ReturnedType => typeof(Guid);

        public new bool Equals(object x, object y)
        {
            return x != null && x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var bytes = (byte[])NHibernateUtil.Binary.NullSafeGet(rs, names[0], session);
            if (bytes == null || bytes.Length == 0)
                return null;

            var reorderedBytes = new byte[16];

            for (var i = 0; i < 16; i++)
                reorderedBytes[_byteOrder[i]] = bytes[i];

            return new Guid(reorderedBytes);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (null != value)
            {
                byte[] bytes = ((Guid)value).ToByteArray();
                var reorderedBytes = new byte[16];

                for (var i = 0; i < 16; i++)
                    reorderedBytes[i] = bytes[_byteOrder[i]];

                NHibernateUtil.Binary.NullSafeSet(cmd, reorderedBytes, index, session);
            }
            else
                NHibernateUtil.Binary.NullSafeSet(cmd, null, index, session);
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public bool IsMutable => false;

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
    }
}
