namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using Subscriptions;

	public class MessageDispatcher<TMessage> : IMessageDispatcher<TMessage> where TMessage : class
	{
		private readonly IObjectBuilder _builder;
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly List<Consumes<TMessage>.All> _consumers = new List<Consumes<TMessage>.All>();

		public MessageDispatcher()
		{
		}

		public MessageDispatcher(IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			_builder = builder;
			_bus = bus;
			_cache = cache;
		}

		public bool Accept(TMessage message)
		{
			foreach (Consumes<TMessage>.All consumer in _consumers)
			{
				Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
				if (selectiveConsumer != null)
				{
					// if the consumer is selective, ask if they want it and if so return true
					if (selectiveConsumer.Accept(message))
						return true;
				}
				else
				{
					// they aren't selective, so return true
					return true;
				}
			}

			return false;
		}

		public void Consume(TMessage message)
		{
			IList<Consumes<TMessage>.All> consumers = new List<Consumes<TMessage>.All>(_consumers.Count);

			foreach (Consumes<TMessage>.All consumer in _consumers)
			{
				Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
				if (selectiveConsumer != null)
				{
					if (selectiveConsumer.Accept(message))
					{
						consumers.Add(consumer);
					}
				}
				else
				{
					consumers.Add(consumer);
				}
			}

			foreach (Consumes<TMessage>.All consumer in consumers)
			{
				consumer.Consume(message);
			}
		}

		public bool Accept(object obj)
		{
			TMessage message = obj as TMessage;
			if (message == null)
				throw new ArgumentException("The message is not of type " + typeof (TMessage).FullName, "obj");

			return Accept(message);
		}

		public void Consume(object obj)
		{
			TMessage message = obj as TMessage;
			if (message == null)
				throw new ArgumentException("The message is not of type " + typeof (TMessage).FullName, "obj");

			Consume(message);
		}

		public bool Active
		{
			get { return _consumers.Count > 0; }
		}

		public void Dispose()
		{
			_consumers.Clear();
		}

		public void Attach(Consumes<TMessage>.All consumer)
		{
			lock (_consumers)
			{
				if (!_consumers.Contains(consumer))
				{
					_consumers.Add(consumer);
					if (_cache != null)
						_cache.Add(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
				}
			}
		}

		public void Detach(Consumes<TMessage>.All consumer)
		{
			lock (_consumers)
			{
				if (_consumers.Contains(consumer))
				{
					_consumers.Remove(consumer);
					if (_consumers.Count == 0)
					{
						if (_cache != null)
							_cache.Remove(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
					}
				}
			}
		}
	}
}