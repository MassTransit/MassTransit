namespace MassTransit.ServiceBus.Internal
{
	using System;
	using Exceptions;

	public class CorrelatedSubscription<TComponent, TMessage, TKey> :
		IMessageTypeSubscription
		where TComponent : class, Consumes<TMessage>.All
		where TMessage : class, CorrelatedBy<TKey>

	{
		private readonly Type _messageType;
		private readonly CorrelationIdDispatcher<TMessage, TKey> _componentConsumer;
		private readonly Consumes<TMessage>.Selected _selectiveConsumer;

		public CorrelatedSubscription(IObjectBuilder builder)
		{
			_messageType = typeof(TMessage);

			_componentConsumer = new CorrelationIdDispatcher<TMessage, TKey>(builder);
			_selectiveConsumer = new SelectiveComponentDispatcher<TComponent, TMessage>(builder);
		}

		public void Subscribe<T>(MessageTypeDispatcher dispatcher, T component) where T : class
		{
			Consumes<TMessage>.All consumer = component as Consumes<TMessage>.All;
			if (consumer == null)
				throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof(T), _messageType));

			Subscribe(dispatcher, consumer);
		}

		public void Subscribe(MessageTypeDispatcher dispatcher, Consumes<TMessage>.All consumer)
		{
			IMessageDispatcher<TMessage> messageDispatcher = dispatcher.GetMessageProducer<TMessage>();

			messageDispatcher.Attach(_componentConsumer);

			_componentConsumer.Attach(consumer);
		}

		public void Unsubscribe<T>(MessageTypeDispatcher dispatcher, T component) where T : class
		{
			Consumes<TMessage>.All consumer = component as Consumes<TMessage>.All;
			if (consumer == null)
				throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof(T), _messageType));

			Unsubscribe(dispatcher, consumer);
		}

		public void Unsubscribe(MessageTypeDispatcher dispatcher, Consumes<TMessage>.All consumer)
		{
			_componentConsumer.Detach(consumer);

			// TODO detach if no longer interested in any subscribers
		}

		public void AddComponent(MessageTypeDispatcher dispatcher)
		{
			IMessageDispatcher<TMessage> messageDispatcher = dispatcher.GetMessageProducer<TMessage>();

			messageDispatcher.Attach(_selectiveConsumer);
		}

		public void RemoveComponent(MessageTypeDispatcher dispatcher)
		{
			IMessageDispatcher<TMessage> messageDispatcher = dispatcher.GetMessageProducer<TMessage>();

			messageDispatcher.Detach(_selectiveConsumer);
		}
	}
}