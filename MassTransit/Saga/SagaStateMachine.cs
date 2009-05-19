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
namespace MassTransit.Saga
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Runtime.Serialization;
	using Magnum.CollectionExtensions;
	using Magnum.StateMachine;

	public class SagaStateMachine<T> :
		StateMachine<T>
		where T : SagaStateMachine<T>
	{
		private static readonly Dictionary<Event, EventBinder<T>> _binders = new Dictionary<Event, EventBinder<T>>();

		protected SagaStateMachine()
		{
		}

		public SagaStateMachine(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected static EventBinder<T,V> Correlate<V>(Event<V> targetEvent)
		{
			return (EventBinder<T,V>)_binders.Retrieve(targetEvent, () => new DataEventBinder<T, V>());
		}
	}

	public interface EventBinder<TSaga>
	{
		Expression<Func<TSaga, T, bool>> Bind<T>();
	}

	public interface EventBinder<TSaga,TMessage> :
		EventBinder<TSaga>
	{
		void By(Expression<Func<TSaga, TMessage, bool>> expression);
	}

	public class DataEventBinder<TSaga, TMessage> :
		EventBinder<TSaga,TMessage>
	{
		private Expression<Func<TSaga, TMessage, bool>> _expression;

		public void By(Expression<Func<TSaga, TMessage, bool>> expression)
		{
			_expression = expression;
		}

		public Expression<Func<TSaga, T, bool>> Bind<T>()
		{
			var sagaParameter = Expression.Parameter(typeof (TSaga), "saga");
			var messageParameter = Expression.Parameter(typeof (T), "message");
			var cast = Expression.Convert(messageParameter, typeof (TMessage));

			return Expression.Lambda<Func<TSaga, T, bool>>(Expression.Invoke(_expression, sagaParameter, cast), new[]{sagaParameter,messageParameter});
		}
	}
}