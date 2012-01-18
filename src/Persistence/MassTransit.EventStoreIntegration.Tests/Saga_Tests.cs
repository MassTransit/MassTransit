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
namespace MassTransit.EventStoreIntegration.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Threading;
	using EventStore;
	using Magnum.StateMachine;
	using MassTransit.Tests.TextFixtures;
	using Saga;
	using Util;

	public class Saga_Tests
		: LoopbackTestFixture
	{
		ISagaRepository<ConcurrentSaga> _sagaRepository;

		protected override void EstablishContext()
		{
			base.EstablishContext();



			_sagaRepository = new EventStoreRepository<ConcurrentSaga>(
				);
		}
	}
	public interface IRouteEvents
	{
		void Register<T>(Action<T> handler);
		void Register(IEventSourcedSaga aggregate);
		void Dispatch(object eventMessage);
	}
	public class ConventionEventRouter : IRouteEvents
	{
		private readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();
		private IEventSourcedSaga registered;

		public virtual void Register<T>([NotNull] Action<T> handler)
		{
			if (handler == null) throw new ArgumentNullException("handler");
			Register(typeof(T), @event => handler((T)@event));
		}

		public virtual void Register([NotNull] IEventSourcedSaga aggregate)
		{
			if (aggregate == null) throw new ArgumentNullException("aggregate");

			registered = aggregate;

			// Get instance methods named Apply with one parameter returning void
			var applyMethods = aggregate.GetType()
				.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(m => m.Name == "Apply" && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof(void))
				.Select(m => new
				{
					Method = m,
					MessageType = m.GetParameters().Single().ParameterType
				});

			foreach (var apply in applyMethods)
			{
				var applyMethod = apply.Method;
				_handlers.Add(apply.MessageType, m => applyMethod.Invoke(aggregate, new[] { m }));
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

				throw new HandlerForDomainEventNotFoundException(exceptionMessage);
			}
		}

		private void Register(Type messageType, Action<object> handler)
		{
			_handlers[messageType] = handler;
		}
	}

	/// <summary>
	/// Custom helper for event sourced sagas.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class EventSourcedSaga<T>
		: SagaStateMachine<T>, 
		  IEventSourcedSaga
		where T : EventSourcedSaga<T>
	{
		private readonly ICollection<object> _uncommittedEvents = new LinkedList<object>();
		private readonly IRouteEvents _registeredRoutes = new ConventionEventRouter();

		public ulong Version { get; private set; }

		IEnumerable<object> IEventSourcedSaga.GetUncommittedEvents()
		{
			return _uncommittedEvents;
		}
		void IEventSourcedSaga.ClearUncommittedEvents()
		{
			_uncommittedEvents.Clear();
		}

		protected void Transition(object message)
		{
			_uncommittedEvents.Add(message);
		}
		//ICollection IEventSourcedSaga.GetUndispatchedMessages()
		//{
		//    return this.undispatched as ICollection;
		//}
		//void IEventSourcedSaga.ClearUndispatchedMessages()
		//{
		//    this.undispatched.Clear();
		//}

		public override int GetHashCode()
		{
			return this.CorrelationId.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as IEventSourcedSaga);
		}

		public virtual bool Equals(IEventSourcedSaga other)
		{
			return null != other && other.CorrelationId == CorrelationId;
		}

		public abstract Guid CorrelationId { get; protected set; }
		public abstract IServiceBus Bus { get; set; }
	}

	public class ConcurrentSaga :
		SagaStateMachine<ConcurrentSaga>
	{
		static ConcurrentSaga()
		{
			SagaStateMachine<ConcurrentSaga>.Define(() =>
			{
				Initially(
					When(Start)
						.Then((saga, message) =>
						{
							Trace.WriteLine("Consuming " + message.GetType());
							Thread.Sleep(3000);
							saga.Name = message.Name;
							saga.Value = message.Value;
							Trace.WriteLine("Completed " + message.GetType());
						}).TransitionTo(Active));

				During(Active,
					When(Continue)
						.Then((saga, message) =>
						{
							Trace.WriteLine("Consuming " + message.GetType());
							Thread.Sleep(1000);
							saga.Value = message.Value;
							Trace.WriteLine("Completed " + message.GetType());
						}).Complete());
			});
		}

		public ConcurrentSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected ConcurrentSaga()
		{
		}

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State Active { get; set; }

		public static Event<StartConcurrentSaga> Start { get; set; }
		public static Event<ContinueConcurrentSaga> Continue { get; set; }

		public string Name { get; set; }
		public int Value { get; set; }
		public Guid CorrelationId { get; set; }
		public IServiceBus Bus { get; set; }
	}

	public class ContinueConcurrentSaga :
		CorrelatedBy<Guid>
	{
		public int Value { get; set; }
		public virtual Guid CorrelationId { get; set; }
	}

	public class StartConcurrentSaga :
		CorrelatedBy<Guid>
	{
		public string Name { get; set; }

		public int Value { get; set; }
		public virtual Guid CorrelationId { get; set; }
	}
}