// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.NHibernateIntegration
{
    using System;
    using System.Data;
    using NHibernate;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;

    // taken from https://gist.github.com/3075912
    public class BinaryGuidType :
        IUserType
    {
        static readonly int[] _byteOrder = new[] {10, 11, 12, 13, 14, 15, 8, 9, 6, 7, 4, 5, 0, 1, 2, 3};
        static readonly SqlType[] _types = new[] {new SqlType(DbType.Binary)};

        public SqlType[] SqlTypes
        {
            get { return _types; }
        }

        public Type ReturnedType
        {
            get { return typeof(Guid); }
        }

        public new bool Equals(object x, object y)
        {
            return (x != null && x.Equals(y));
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var bytes = (byte[])NHibernateUtil.Binary.NullSafeGet(rs, names[0]);
            if (bytes == null || bytes.Length == 0)
                return null;

            var reorderedBytes = new byte[16];

            for (int i = 0; i < 16; i++)
                reorderedBytes[_byteOrder[i]] = bytes[i];

            return new Guid(reorderedBytes);
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (null != value)
            {
                byte[] bytes = ((Guid)value).ToByteArray();
                var reorderedBytes = new byte[16];

                for (int i = 0; i < 16; i++)
                    reorderedBytes[i] = bytes[_byteOrder[i]];

                NHibernateUtil.Binary.NullSafeSet(cmd, reorderedBytes, index);
            }
            else
                NHibernateUtil.Binary.NullSafeSet(cmd, null, index);
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public bool IsMutable
        {
            get { return false; }
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
    }
}