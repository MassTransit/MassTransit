/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Internal
{
	using System;
	using Exceptions;
	using Subscriptions;

	public class MessageTypeSubscription<TComponent, TMessage> :
		ISubscriptionTypeInfo
		where TComponent : class, Consumes<TMessage>.All
		where TMessage : class
	{
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly Consumes<TMessage>.Selected _componentConsumer;
		private readonly IMessageTypeDispatcher _dispatcher;
		private readonly Type _messageType;

		public MessageTypeSubscription(SubscriptionMode mode, IMessageTypeDispatcher dispatcher, IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			_dispatcher = dispatcher;
			_bus = bus;
			_cache = cache;
			_messageType = typeof (TMessage);

			if (mode == SubscriptionMode.Selected)
				_componentConsumer = new SelectiveComponentDispatcher<TComponent, TMessage>(builder);
			else
				_componentConsumer = new ComponentDispatcher<TComponent, TMessage>(builder);
		}

		public void Subscribe<T>(T component) where T : class
		{
			Consumes<TMessage>.All consumer = component as Consumes<TMessage>.All;
			if (consumer == null)
				throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof (T), _messageType));

			Subscribe(consumer);
		}

		public void Unsubscribe<T>(T component) where T : class
		{
			Consumes<TMessage>.All consumer = component as Consumes<TMessage>.All;
			if (consumer == null)
				throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof (T), _messageType));

			Unsubscribe(consumer);
		}

		public void AddComponent()
		{
			_dispatcher.Attach<TMessage>(_componentConsumer);
		}

		public void RemoveComponent()
		{
			_dispatcher.Detach<TMessage>(_componentConsumer);
		}

		public void Subscribe(Consumes<TMessage>.All consumer)
		{
			_dispatcher.Attach<TMessage>(consumer);

			if (_cache != null)
				_cache.Add(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
		}

		public void Unsubscribe(Consumes<TMessage>.All consumer)
		{
			_dispatcher.Detach<TMessage>(consumer);
		}
	}
}