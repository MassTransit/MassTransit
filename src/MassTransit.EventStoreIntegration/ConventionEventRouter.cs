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
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using EventStore;
	using Util;

	public class ConventionEventRouter : IRouteEvents
	{
		readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();
		ISagaEventSourced registered;

		public virtual void Register<T>([NotNull] Action<T> handler)
		{
			if (handler == null) throw new ArgumentNullException("handler");
			Register(typeof (T), @event => handler((T) @event));
		}

		public virtual void Register([NotNull] ISagaEventSourced aggregate)
		{
			if (aggregate == null) throw new ArgumentNullException("aggregate");

			registered = aggregate;

			// Get instance methods named Apply with one parameter returning void
			var applyMethods = aggregate.GetType()
				.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(m => m.Name == "Apply" && m.GetParameters().Length == 1
				            && m.ReturnParameter.ParameterType == typeof (void))
				.Select(m => new
					{
						Method = m,
						MessageType = m.GetParameters().Single().ParameterType
					});

			foreach (var apply in applyMethods)
			{
				var applyMethod = apply.Method;
				_handlers.Add(apply.MessageType, m => applyMethod.Invoke(aggregate, new[] {m}));
			}
		}

		public virtual void Dispatch(object eventMessage)
		{
			if (eventMessage == null)
				throw new ArgumentNullException("eventMessage");

			Action<object> handler;
			if (_handlers.TryGetValue(eventMessage.GetType(), out handler))
				handler(eventMessage);
			else
			{
				var exceptionMessage = "Aggregate of type '{0}' raised an event of type '{1}' but not handler could be found to handle the message."
					.FormatWith(registered.GetType().Name, eventMessage.GetType().Name);

				throw new ApplicationException(exceptionMessage);
			}
		}

		void Register(Type messageType, Action<object> handler)
		{
			_handlers[messageType] = handler;
		}
	}
}