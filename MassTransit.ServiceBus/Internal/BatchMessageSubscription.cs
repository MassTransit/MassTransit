namespace MassTransit.ServiceBus.Internal
{
	using System;
	using Exceptions;
	using Subscriptions;

	public class BatchMessageSubscription<TComponent, TMessage, TBatchId> :
		ISubscriptionTypeInfo
		where TComponent : class, Consumes<Batch<TMessage, TBatchId>>.Selected
		where TMessage : class, BatchedBy<TBatchId>
	{
		private readonly BatchDistributor<TMessage, TBatchId> _batchDispatcher;
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly Consumes<Batch<TMessage, TBatchId>>.Selected _componentConsumer;
		private readonly IMessageTypeDispatcher _dispatcher;
		private readonly Type _messageType;

		public BatchMessageSubscription(IMessageTypeDispatcher dispatcher, IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			_dispatcher = dispatcher;
			_bus = bus;
			_cache = cache;
			_messageType = typeof (TMessage);

			_batchDispatcher = new BatchDistributor<TMessage, TBatchId>(bus);
			_componentConsumer = new SelectiveComponentDispatcher<TComponent, Batch<TMessage, TBatchId>>(builder);
		}

		public void Subscribe<T>(T component) where T : class
		{
			Consumes<Batch<TMessage, TBatchId>>.All consumer = component as Consumes<Batch<TMessage, TBatchId>>.All;
			if (consumer == null)
				throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof (T), _messageType));

			Subscribe(consumer);
		}

		public void Unsubscribe<T>(T component) where T : class
		{
			Consumes<Batch<TMessage, TBatchId>>.All consumer = component as Consumes<Batch<TMessage, TBatchId>>.All;
			if (consumer == null)
				throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof (T), _messageType));

			Unsubscribe(consumer);
		}

		public void AddComponent()
		{
			_batchDispatcher.Attach(_componentConsumer);
			_dispatcher.Attach<TMessage>(_batchDispatcher);

			if (_cache != null)
				_cache.Add(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
		}

		public void RemoveComponent()
		{
			_batchDispatcher.Detach(_componentConsumer);

			DetachIfInactive();
		}

		public void Dispose()
		{
		}

		public void Subscribe(Consumes<Batch<TMessage, TBatchId>>.All consumer)
		{
			_batchDispatcher.Attach(consumer);
			_dispatcher.Attach<TMessage>(_batchDispatcher);

			if (_cache != null)
				_cache.Add(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
		}

		public void Unsubscribe(Consumes<Batch<TMessage, TBatchId>>.All consumer)
		{
			_batchDispatcher.Detach(consumer);

			DetachIfInactive();
		}

		private void DetachIfInactive()
		{
			if (_batchDispatcher.Active == false)
			{
				_dispatcher.Detach<TMessage>(_batchDispatcher);
				if (_dispatcher.GetMessageDispatcher<TMessage>().Active == false)
				{
					if (_cache != null)
						_cache.Remove(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
				}
			}
		}
	}
}