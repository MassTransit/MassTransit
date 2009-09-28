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
namespace MassTransit.Saga.Configuration
{
	using System;
	using System.Collections.Generic;
	using Magnum.Collections;
	using Magnum.Reflection;
	using Magnum.StateMachine;

	public class SagaStateMachineEventInspector<TSaga> :
		ReflectiveVisitorBase<SagaStateMachineEventInspector<TSaga>>,
		IStateMachineInspector
		where TSaga : SagaStateMachine<TSaga>, ISaga
	{
		private readonly MultiDictionary<SagaEvent<TSaga>, State> _events = new MultiDictionary<SagaEvent<TSaga>, State>(false);
		private State _currentState;

		public SagaStateMachineEventInspector()
			: base("Inspect")
		{
		}

		public void Inspect(object obj)
		{
			base.Visit(obj);
		}

		public void Inspect(object obj, Action action)
		{
			base.Visit(obj, () =>
				{
					action();
					return true;
				});
		}

		public bool Inspect<T>(T machine)
			where T : SagaStateMachine<T>
		{
			return true;
		}

		public bool Inspect<T>(State<T> state)
			where T : SagaStateMachine<T>
		{
			_currentState = state;

			return true;
		}

		public bool Inspect<T>(BasicEvent<T> eevent)
			where T : SagaStateMachine<T>
		{
			return true;
		}

		public bool Inspect<T, V>(DataEvent<T, V> eevent)
			where T : SagaStateMachine<T>, ISaga
		{
			SagaEvent<TSaga> sagaEvent = new SagaEvent<TSaga>(eevent, typeof (V));

			_events.Add(sagaEvent, _currentState);

			return true;
		}

		public IEnumerable<EventInspectorResult<TSaga>> GetResults()
		{
			foreach (var eventStates in _events)
			{
				yield return new EventInspectorResult<TSaga>(eventStates.Key, eventStates.Value);
			}
		}
	}
}