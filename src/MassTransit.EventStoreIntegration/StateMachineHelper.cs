// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.EventStoreIntegration
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.StateMachine;
	using Magnum.Threading;
	using Saga;

	public static class StateMachineHelper
	{
		private static readonly ReaderWriterLockedDictionary<Type, Func<string, State>> _getStateMethods;

		static StateMachineHelper()
		{
			_getStateMethods = new ReaderWriterLockedDictionary<Type, Func<string, State>>();
		}

		public static void SetCurrentState<TSaga>(this TSaga saga, State state)
			where TSaga : SagaStateMachine<TSaga>
		{
			var newState = State<TSaga>.GetState(state);

			FieldInfo field = typeof(StateMachine<TSaga>)
				.GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

			field.SetValue(saga, newState);
		}

		public static Func<string, State> GetStateMethod(object owner)
		{
			Type ownerType = owner.GetType();

			return _getStateMethods.Retrieve(ownerType, () =>
			{
				Type stateMachineType = ownerType;
				while (stateMachineType != typeof(object))
				{
					if (stateMachineType.IsGenericType && stateMachineType.GetGenericTypeDefinition() == typeof(StateMachine<>))
						break;

					stateMachineType = stateMachineType.BaseType;
				}

				if (stateMachineType == typeof(object))
					throw new StateMachineException("The owner type is not a state machine: " + ownerType.FullName);

				MethodInfo mi = stateMachineType.GetMethod("GetState", BindingFlags.Static | BindingFlags.Public);
				if (mi == null)
					throw new StateMachineException("Could not find GetState method on " + ownerType.FullName);

				var input = Expression.Parameter(typeof(string), "input");
				var method = Expression.Call(mi, input);

				Func<string, State> invoker = Expression.Lambda<Func<string, State>>(method, input).Compile();
				return invoker;
			});
		}
	}
}