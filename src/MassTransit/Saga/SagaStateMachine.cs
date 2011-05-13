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
namespace MassTransit.Saga
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using System.Runtime.Serialization;
	using Configuration;
	using Context;
	using Magnum.Extensions;
	using Magnum.StateMachine;

	[DebuggerDisplay("{CurrentState} - {typeof(T).Name}")]
	public class SagaStateMachine<T> :
		StateMachine<T>
		where T : SagaStateMachine<T>
	{
		private static readonly Dictionary<Event, EventBinder<T>> _binders = new Dictionary<Event, EventBinder<T>>();
		private static Expression<Func<T, bool>> _completedExpression = x => false;

		protected SagaStateMachine()
		{
		}

		public SagaStateMachine(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public static bool TryGetCorrelationExpressionForEvent<TMessage>(Event targetEvent, out Expression<Func<T, TMessage, bool>> expression)
		{
			if (_binders.ContainsKey(targetEvent))
			{
				expression = _binders[targetEvent].GetBindExpression<TMessage>();
				return true;
			}

			expression = null;
			return false;
		}

		public static Expression<Func<T,bool>> GetCompletedExpression()
		{
			return _completedExpression;
		}

		protected static void RemoveWhen(Expression<Func<T, bool>> expression)
		{
			_completedExpression = expression;
		}

		protected static EventBinder<T, V> Correlate<V>(Event<V> targetEvent)
		{
			return (EventBinder<T, V>) _binders.Retrieve(targetEvent, () => new DataEventBinder<T, V>());
		}
	}

	public static class ExtensionsToStateMachine
	{
		public static DataEventAction<T, TData> Publish<T, TData, TMessage>(this DataEventAction<T, TData> eventAction, Func<T, TData, TMessage> action)
			where T : SagaStateMachine<T>, ISaga
			where TData : class 
			where TMessage : class
		{
			eventAction.Call((saga, message) => saga.Bus.Publish(action(saga, message)));
			return eventAction;
		}

		public static DataEventAction<T, TData> RespondWith<T, TData, TMessage>(this DataEventAction<T, TData> eventAction, Func<T, TData, TMessage> action)
			where T : SagaStateMachine<T>, ISaga
			where TData : class 
			where TMessage : class
		{
			eventAction.Call((saga, message) => ContextStorage.Context().Respond(action(saga, message)));
			return eventAction;
		}

		public static DataEventAction<T, TData> RetryLater<T, TData>(this DataEventAction<T, TData> eventAction)
			where T : SagaStateMachine<T>, ISaga
			where TData : class
		{
			eventAction.Call((saga, message) => ContextStorage.Context().RetryLater());
			return eventAction;
		}

		public static void EnumerateDataEvents<T>(this T saga, Action<Type> messageAction)
			where T : SagaStateMachine<T>, ISaga
		{
			var inspector = new SagaStateMachineEventInspector<T>();
			saga.Inspect(inspector);

			inspector.GetResults().Each(x =>
			{
				messageAction(x.SagaEvent.MessageType);
			});
		}

		public static void EnumerateDataEvents<T>(this T saga, Action<SagaEvent<T>, IEnumerable<State>> messageAction)
			where T : SagaStateMachine<T>, ISaga
		{
			var inspector = new SagaStateMachineEventInspector<T>();
			saga.Inspect(inspector);

			inspector.GetResults().Each(x =>
			{
				messageAction(x.SagaEvent, x.States);
			});
		}
	}
}