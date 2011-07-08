// Copyright 2007-2008 The Apache Software Foundation.
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
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.StateMachine;
	using NHibernate;
	using NHibernate.SqlTypes;
	using NHibernate.UserTypes;
	using Magnum.Threading;

	/// <summary>
	/// An NHibernate user type for persisting state machines current state
	/// </summary>
	[Serializable]
	public class StateMachineUserType :
		IUserType
	{
		private static readonly ReaderWriterLockedDictionary<Type, Func<string, State>> _getStateMethods;

		static StateMachineUserType()
		{
			_getStateMethods = new ReaderWriterLockedDictionary<Type, Func<string, State>>();
		}

		bool IUserType.Equals(object x, object y)
		{
			State xs = (State) x;
			State ys = (State) y;

			return xs.Name.Equals(ys.Name);
		}

		public int GetHashCode(object x)
		{
			return ((State) x).Name.GetHashCode();
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			string value = (string) NHibernateUtil.String.NullSafeGet(rs, names);

			var method = GetStateMethod(owner);

			State state = method(value);

			return state;
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			if (value == null)
			{
				NHibernateUtil.String.NullSafeSet(cmd, null, index);
				return;
			}

			value = ((State) value).Name;

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

		public SqlType[] SqlTypes
		{
			get { return new[] {NHibernateUtil.String.SqlType}; }
		}

		public Type ReturnedType
		{
			get { return typeof (State); }
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public static Func<string, State> GetStateMethod(object owner)
		{
			Type ownerType = owner.GetType();

			return _getStateMethods.Retrieve(ownerType, () =>
				{
					Type stateMachineType = ownerType;
					while (stateMachineType != typeof (object))
					{
						if (stateMachineType.IsGenericType && stateMachineType.GetGenericTypeDefinition() == typeof (StateMachine<>))
							break;

						stateMachineType = stateMachineType.BaseType;
					}

					if (stateMachineType == typeof (object))
						throw new StateMachineException("The owner type is not a state machine: " + ownerType.FullName);

					MethodInfo mi = stateMachineType.GetMethod("GetState", BindingFlags.Static | BindingFlags.Public);
					if (mi == null)
						throw new StateMachineException("Could not find GetState method on " + ownerType.FullName);

					var input = Expression.Parameter(typeof (string), "input");
					var method = Expression.Call(mi, input);

					Func<string, State> invoker = Expression.Lambda<Func<string, State>>(method, input).Compile();
					return invoker;
				});
		}
	}
}