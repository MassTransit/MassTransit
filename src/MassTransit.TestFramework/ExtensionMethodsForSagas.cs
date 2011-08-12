// Copyright 2007-2010 The Apache Software Foundation.
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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading;
	using Magnum.Extensions;
	using Magnum.StateMachine;
	using NUnit.Framework;
	using Saga;

	public static class ExtensionMethodsForSagas
	{
		public static TimeSpan Timeout { get; set; }

		static ExtensionMethodsForSagas()
		{
			Timeout = 8.Seconds();
		}

		public static TSaga ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId)
			where TSaga : class, ISaga
		{
			return ShouldContainSaga(repository, sagaId, Timeout);
		}

		public static TSaga ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId, TimeSpan timeout)
			where TSaga : class, ISaga
		{
			DateTime giveUpAt = DateTime.Now + timeout;

			while (DateTime.Now < giveUpAt)
			{
				TSaga saga = repository.Where(x => x.CorrelationId == sagaId).FirstOrDefault();
				if (saga != null)
				{
					return saga;
				}

				Thread.Sleep(100);
			}

			return null;
		}

		public static TSaga ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Expression<Func<TSaga, bool>> filter)
			where TSaga : class, ISaga
		{
			return ShouldContainSaga(repository, filter, Timeout);
		}

		public static TSaga ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Expression<Func<TSaga, bool>> filter, TimeSpan timeout)
			where TSaga : class, ISaga
		{
			DateTime giveUpAt = DateTime.Now + timeout;

			var sagaFilter = new SagaFilter<TSaga>(filter);

			while (DateTime.Now < giveUpAt)
			{
				List<TSaga> sagas = repository.Where(sagaFilter).ToList();
				if (sagas.Count > 0)
					return sagas.Single();

				Thread.Sleep(100);
			}

			return null;
		}

		public static void ShouldBeInState<TSaga>(this TSaga saga, State state)
			where TSaga : SagaStateMachine<TSaga>
		{
			ShouldBeInState(saga, state, Timeout);
		}

		public static void ShouldBeInState<TSaga>(this TSaga saga, State state, TimeSpan timeout)
			where TSaga : SagaStateMachine<TSaga>
		{
			DateTime giveUpAt = DateTime.Now + timeout;

			while (DateTime.Now < giveUpAt)
			{
				if (saga.CurrentState == state)
					return;

				Thread.Sleep(100);
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