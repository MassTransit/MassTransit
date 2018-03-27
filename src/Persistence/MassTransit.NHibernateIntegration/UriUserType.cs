// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
			string value = (string) NHibernateUtil.String.NullSafeGet(rs, names, session);

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

		public object DeepCopy(object value) => value ?? null;

		public object Replace(object original, object target, object owner) => original;

		public object Assemble(object cached, object owner) => cached;

		public object Disassemble(object value) => value;

		public SqlType[] SqlTypes => new[] {NHibernateUtil.String.SqlType};

	    public Type ReturnedType => typeof (Uri);

	    public bool IsMutable => false;
	}
}