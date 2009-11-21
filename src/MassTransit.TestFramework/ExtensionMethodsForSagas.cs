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
namespace MassTransit.TestFramework
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Threading;
	using Magnum.DateTimeExtensions;
	using Magnum.StateMachine;
	using NUnit.Framework;
	using Saga;

	public static class ExtensionMethodsForSagas
	{
		public static TimeSpan Timeout { get; set; }

		static ExtensionMethodsForSagas()
		{
			Timeout = 2.Seconds();
		}

		public static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>(this IObjectBuilder builder)
			where TSaga : class, ISaga
		{
			var repository = new InMemorySagaRepository<TSaga>();

			builder.Add<ISagaRepository<TSaga>>(repository);

			return repository;
		}

		public static TSaga ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId)
			where TSaga : class, ISaga
		{
			DateTime giveUpAt = DateTime.Now + Timeout;

			while (DateTime.Now < giveUpAt)
			{
				TSaga saga = repository.Where(x => x.CorrelationId == sagaId).FirstOrDefault();
				if (saga != null)
				{
					return saga;
				}

				Thread.Sleep(10);
			}

			return null;
		}

		public static void ShouldBeInState<TSaga>(this TSaga saga, State state)
			where TSaga : SagaStateMachine<TSaga>
		{
			DateTime giveUpAt = DateTime.Now + Timeout;

			while (DateTime.Now < giveUpAt)
			{
				if (saga.CurrentState == state)
					return;

				Thread.Sleep(10);
			}

			Assert.Fail("The saga was not in the expected state: " + state.Name + " (" + saga.CurrentState.Name + ")");
		}

		public static void SetCurrentState<TSaga>(this TSaga saga, State state)
			where TSaga : SagaStateMachine<TSaga>
		{
			var newState = State<TSaga>.GetState(state);

			FieldInfo field = typeof(StateMachine<TSaga>)
				.GetField("_currentState", BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.FlattenHierarchy);

			field.SetValue(saga, newState);
		}
	}
}