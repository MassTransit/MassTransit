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
	using System.Collections.Generic;

	public class CorrelationIdDispatcher<TMessage, TKey> :
		Consumes<TMessage>.Selected,
		Produces<TMessage>
		where TMessage : class, CorrelatedBy<TKey>
	{
		private readonly Dictionary<TKey, MessageDispatcher<TMessage>> _dispatchers = new Dictionary<TKey, MessageDispatcher<TMessage>>();

		public bool Active
		{
			get { return _dispatchers.Count > 0; }
		}

		public void Consume(TMessage message)
		{
			CorrelatedBy<TKey> correlation = message;

			TKey correlationId = correlation.CorrelationId;

			if (_dispatchers.ContainsKey(correlationId))
			{
				_dispatchers[correlationId].Consume(message);
			}
		}

		public bool Accept(TMessage message)
		{
			CorrelatedBy<TKey> correlation = message;

			TKey correlationId = correlation.CorrelationId;

			if (_dispatchers.ContainsKey(correlationId))
			{
				return _dispatchers[correlationId].Accept(message);
			}

			return false;
		}

		public void Attach(Consumes<TMessage>.All consumer)
		{
			Consumes<TMessage>.For<TKey> correlatedConsumer = consumer as Consumes<TMessage>.For<TKey>;
			if (correlatedConsumer == null)
				throw new ArgumentException(string.Format("The object does not support Consumes<{0}>.For<{1}>", typeof (TMessage).Name, typeof (TKey).Name), "consumer");

			Attach(correlatedConsumer);
		}

		public void Detach(Consumes<TMessage>.All consumer)
		{
			Consumes<TMessage>.For<TKey> correlatedConsumer = consumer as Consumes<TMessage>.For<TKey>;
			if (correlatedConsumer == null)
				throw new ArgumentException(string.Format("The object does not support Consumes<{0}>.For<{1}>", typeof (TMessage).Name, typeof (TKey).Name), "consumer");

			Detach(correlatedConsumer);
		}

		public void Consume(object obj)
		{
			Consume((TMessage) obj);
		}

		public void Dispose()
		{
			foreach (MessageDispatcher<TMessage> dispatcher in _dispatchers.Values)
			{
				dispatcher.Dispose();
			}

			_dispatchers.Clear();
		}

		private MessageDispatcher<TMessage> GetDispatcher(TKey correlationId)
		{
			if (_dispatchers.ContainsKey(correlationId))
				return _dispatchers[correlationId];

			lock (_dispatchers)
			{
				if (_dispatchers.ContainsKey(correlationId))
					return _dispatchers[correlationId];

				MessageDispatcher<TMessage> dispatcher = new MessageDispatcher<TMessage>();

				_dispatchers.Add(correlationId, dispatcher);

				// TODO
				//if (_cache != null)
				//	_cache.Add(new Subscription(typeof (TMessage).FullName, correlationId.ToString(), _bus.Endpoint.Uri));

				return dispatcher;
			}
		}

		public void Attach(Consumes<TMessage>.For<TKey> consumer)
		{
			TKey correlationId = consumer.CorrelationId;

			MessageDispatcher<TMessage> dispatcher = GetDispatcher(correlationId);

			dispatcher.Attach(consumer);
		}

		public void Detach(Consumes<TMessage>.For<TKey> consumer)
		{
			TKey correlationId = consumer.CorrelationId;

			MessageDispatcher<TMessage> dispatcher = GetDispatcher(correlationId);

			dispatcher.Detach(consumer);

			if (dispatcher.Active == false)
			{
				lock (_dispatchers)
				{
					if (_dispatchers.ContainsKey(correlationId))
						_dispatchers.Remove(correlationId);
				}
			}
		}
	}
}