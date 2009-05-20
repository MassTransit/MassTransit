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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Exceptions;
	using Magnum.InterfaceExtensions;
	using Magnum.Reflection;
	using Magnum.StateMachine;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using Pipeline;
	using Util;

	public class SagaStateMachineSubscriptionInspector :
		ReflectiveVisitorBase<SagaStateMachineSubscriptionInspector>,
		IStateMachineInspector
	{
		private readonly ISubscriberContext _context;
		private readonly HashSet<Type> _subscribedMessageTypes = new HashSet<Type>();
		private State _currentState;
		private readonly List<UnsubscribeAction> _unsubscribeActions = new List<UnsubscribeAction>();

		public SagaStateMachineSubscriptionInspector(ISubscriberContext context)
			: base("Inspect")
		{
			_context = context;
		}

		public IEnumerable<UnsubscribeAction> Results
		{
			get { return _unsubscribeActions; }
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
			if (_subscribedMessageTypes.Contains(typeof (V)))
				return true;

			if (_context.HasMessageTypeBeenDefined(typeof(V)))
				return true;

			_context.MessageTypeWasDefined(typeof (V));

			UnsubscribeAction result = InvokeConnectMethod(eevent);
			_unsubscribeActions.Add(result);

			_subscribedMessageTypes.Add(typeof(V));

			return true;
		}

		private bool InInitialState()
		{
			return _currentState.Name == "Initial";
		}

		protected virtual UnsubscribeAction Connect<TSink, TComponent, TMessage>(DataEvent<TComponent,TMessage> dataEvent)
			where TMessage : class, CorrelatedBy<Guid>
			where TComponent : SagaStateMachine<TComponent>, ISaga
			where TSink : class, IPipelineSink<TMessage>
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(_context.Pipeline);

			var router = routerConfigurator.FindOrCreate<TMessage>();

			ISagaPolicy<TComponent, TMessage> policy;
			if (InInitialState())
				policy = new InitiatingSagaPolicy<TComponent, TMessage>();
			else
				policy = new ExistingSagaPolicy<TComponent, TMessage>();

			var sink = _context.Builder.GetInstance<TSink>(new Hashtable
			                                               	{
			                                               		{"context", _context},
																{"dataEvent", dataEvent},
                                                                {"policy", policy},
			                                               	});
			if (sink == null)
				throw new ConfigurationException("Could not build the message sink: " + typeof(TSink).ToFriendlyName());

			var result = router.Connect(sink);

			UnsubscribeAction remove = _context.SubscribedTo<TMessage>();

			return () => result() && (router.SinkCount == 0) && remove();
		}

		protected virtual UnsubscribeAction Connect<TSink, TComponent, TMessage>(DataEvent<TComponent,TMessage> dataEvent, Expression<Func<TComponent,TMessage,bool>> selector)
			where TMessage : class
			where TComponent : SagaStateMachine<TComponent>, ISaga
			where TSink : class, IPipelineSink<TMessage>
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(_context.Pipeline);

			var router = routerConfigurator.FindOrCreate<TMessage>();

			ISagaPolicy<TComponent, TMessage> policy;
			if (InInitialState())
				policy = new InitiatingSagaPolicy<TComponent, TMessage>();
			else
				policy = new ExistingSagaPolicy<TComponent, TMessage>();

			var sink = _context.Builder.GetInstance<TSink>(new Hashtable
			                                               	{
			                                               		{"context", _context},
																{"dataEvent", dataEvent},
                                                                {"policy", policy},
																{"selector", selector},
			                                               	});
			if (sink == null)
				throw new ConfigurationException("Could not build the message sink: " + typeof(TSink).ToFriendlyName());

			var result = router.Connect(sink);

			UnsubscribeAction remove = _context.SubscribedTo<TMessage>();

			return () => result() && (router.SinkCount == 0) && remove();
		}

		private UnsubscribeAction InvokeConnectMethod<TComponent, TMessage>(DataEvent<TComponent,TMessage> eevent) 
			where TComponent : SagaStateMachine<TComponent>, ISaga
		{
			Type componentType = typeof (TComponent);
			Type messageType = typeof (TMessage);

			if(messageType.GetInterfaces().Where(x => x == typeof(CorrelatedBy<Guid>)).Count() > 0)
			{
				return InvokeCorrelatedConnectMethod(messageType, componentType, eevent);
			}

			Expression<Func<TComponent,TMessage,bool>> expression;
			if(SagaStateMachine<TComponent>.TryGetCorrelationExpressionForEvent(eevent, out expression))
			{
				return InvokePropertyConnectMethod(messageType, componentType, eevent, expression);
			}

			throw new NotSupportedException("No method to connect to event was found");
		}

		private UnsubscribeAction InvokePropertyConnectMethod<TComponent, TMessage>(Type messageType, Type componentType, DataEvent<TComponent, TMessage> eevent, Expression<Func<TComponent, TMessage, bool>> selector) 
			where TComponent : SagaStateMachine<TComponent>
		{
			Type sinkType = typeof(PropertySagaStateMachineMessageSink<,>).MakeGenericType(componentType, messageType);

			MethodInfo genericMethod = ReflectiveMethodInvoker.FindMethod(GetType(),
				"Connect",
				new[] { sinkType, typeof(TComponent), messageType },
				new[] { typeof(DataEvent<TComponent, TMessage>), typeof(Expression<Func<TComponent,TMessage,bool>>)});

			if (genericMethod == null)
				throw new ConfigurationException(string.Format("Unable to subscribe for type: {0} ({1})",
					typeof(TComponent).FullName, messageType.FullName));

			var target = Expression.Parameter(typeof(SagaStateMachineSubscriptionInspector), "target");
			var dataEvent = Expression.Parameter(typeof(DataEvent<TComponent, TMessage>), "dataEvent");
			var expression = Expression.Parameter(typeof(Expression<Func<TComponent, TMessage, bool>>), "expression");
			var call = Expression.Call(target, genericMethod, dataEvent, expression);

			var connector = Expression.Lambda<Func<SagaStateMachineSubscriptionInspector, DataEvent<TComponent, TMessage>, Expression<Func<TComponent,TMessage,bool>>, UnsubscribeAction>>(call, new[] { target, dataEvent, expression }).Compile();

			return connector(this, eevent, selector);
		}

		private UnsubscribeAction InvokeCorrelatedConnectMethod<TComponent, TMessage>(Type messageType, Type componentType, DataEvent<TComponent, TMessage> eevent) 
			where TComponent : SagaStateMachine<TComponent>
		{
			Type sinkType = typeof (CorrelatedSagaStateMachineMessageSink<,>).MakeGenericType(componentType, messageType);

			MethodInfo genericMethod = ReflectiveMethodInvoker.FindMethod(GetType(),
				"Connect",
				new[] {sinkType, typeof (TComponent), messageType},
				new[] {typeof(DataEvent<TComponent,TMessage>)});

			if (genericMethod == null)
				throw new ConfigurationException(string.Format("Unable to subscribe for type: {0} ({1})",
					typeof (TComponent).FullName, messageType.FullName));

			var target = Expression.Parameter(typeof(SagaStateMachineSubscriptionInspector), "target");
			var dataEvent = Expression.Parameter(typeof(DataEvent<TComponent,TMessage>), "dataEvent");
			var call = Expression.Call(target, genericMethod, dataEvent);

			var connector = Expression.Lambda<Func<SagaStateMachineSubscriptionInspector, DataEvent<TComponent,TMessage>, UnsubscribeAction>>(call, new[] { target, dataEvent }).Compile();

			return connector(this, eevent);
		}
	}
}