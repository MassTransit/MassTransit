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
	using NHibernate;
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

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			string value = (string) NHibernateUtil.String.NullSafeGet(rs, names);

			Uri uri = new Uri(value);

			return uri;
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			if (value == null)
			{
				NHibernateUtil.String.NullSafeSet(cmd, null, index);
				return;
			}

			value = value.ToString();

			NHibernateUtil.String.NullSafeSet(cmd, value, index);
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

	    public Type ReturnedType => typeof (Uri);

	    public bool IsMutable => false;
	}
}