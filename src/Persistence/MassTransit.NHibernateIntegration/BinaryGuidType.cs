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

        public new bool Equals(object x, object y) => x != null && x.Equals(y);

        public int GetHashCode(object x) => x.GetHashCode();
        
        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var bytes = (byte[])NHibernateUtil.Binary.NullSafeGet(rs, names[0], session);
            if (bytes == null || bytes.Length == 0)
                return null;

            var reorderedBytes = new byte[16];

            for (int i = 0; i < 16; i++)
                reorderedBytes[_byteOrder[i]] = bytes[i];

            return new Guid(reorderedBytes);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (null != value)
            {
                byte[] bytes = ((Guid)value).ToByteArray();
                var reorderedBytes = new byte[16];

                for (int i = 0; i < 16; i++)
                    reorderedBytes[i] = bytes[_byteOrder[i]];

                NHibernateUtil.Binary.NullSafeSet(cmd, reorderedBytes, index, session);
            }
            else
                NHibernateUtil.Binary.NullSafeSet(cmd, null, index, session);
        }

        public object DeepCopy(object value) => value;

        public bool IsMutable => false;

        public object Replace(object original, object target, object owner) => original;

        public object Assemble(object cached, object owner) => cached;

        public object Disassemble(object value) => value;
    }
}