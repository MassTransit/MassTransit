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
namespace MassTransit.Saga.Pipeline
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using Exceptions;
	using Magnum.Reflection;
	using Magnum.StateMachine;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Configuration.Subscribers;
	using Util;

	public class SagaStateMachineSubscriber :
		ReflectiveVisitorBase<SagaStateMachineSubscriber>,
		IStateMachineInspector
	{
		private readonly ISubscriberContext _context;
		private readonly HashSet<Type> _subscribedMessageTypes = new HashSet<Type>();
		private State _currentState;
		private readonly List<UnsubscribeAction> _unsubscribeActions = new List<UnsubscribeAction>();

		public SagaStateMachineSubscriber(ISubscriberContext context)
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
			where T : StateMachine<T>
		{
			return true;
		}

		public bool Inspect<T>(State<T> state)
			where T : StateMachine<T>
		{
			_currentState = state;

			return true;
		}

		public bool Inspect<T>(BasicEvent<T> eevent)
			where T : StateMachine<T>
		{
			return true;
		}

		public bool Inspect<T, V>(DataEvent<T, V> eevent)
			where T : StateMachine<T>
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

			var sink = _context.Builder.GetInstance<TSink>(new Hashtable {{"context", _context},{"dataEvent", dataEvent}});
			if (sink == null)
				throw new ConfigurationException("Could not build the message sink: " + typeof(TSink).ToFriendlyName());

			var result = router.Connect(sink);

			UnsubscribeAction remove = _context.SubscribedTo<TMessage>();

			return () => result() && (router.SinkCount == 0) && remove();
		}

		private UnsubscribeAction InvokeConnectMethod<TComponent, TMessage>(DataEvent<TComponent,TMessage> eevent) 
			where TComponent : StateMachine<TComponent>
		{
			Type componentType = typeof (TComponent);
			Type messageType = typeof (TMessage);

			Type sinkType;
			if (InInitialState())
				sinkType = typeof (InitiateSagaStateMachineSink<,>).MakeGenericType(componentType, messageType);
			else
				sinkType = typeof(OrchestrateSagaStateMachineSink<,>).MakeGenericType(componentType, messageType);

			MethodInfo genericMethod = ReflectiveMethodInvoker.FindMethod(GetType(),
				"Connect",
				new[] {sinkType, typeof (TComponent), messageType},
				new[] {typeof(DataEvent<TComponent,TMessage>)});

			if (genericMethod == null)
				throw new ConfigurationException(string.Format("Unable to subscribe for type: {0} ({1})",
					typeof (TComponent).FullName, messageType.FullName));

			var target = Expression.Parameter(typeof(SagaStateMachineSubscriber), "target");
			var dataEvent = Expression.Parameter(typeof(DataEvent<TComponent,TMessage>), "dataEvent");
			var call = Expression.Call(target, genericMethod, dataEvent);

			var connector = Expression.Lambda<Func<SagaStateMachineSubscriber, DataEvent<TComponent,TMessage>, UnsubscribeAction>>(call, new[] { target, dataEvent }).Compile();

			return connector(this, eevent);
		}
	}
}