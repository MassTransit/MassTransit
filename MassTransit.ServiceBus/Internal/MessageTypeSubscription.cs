namespace MassTransit.ServiceBus.Internal
{
	using System;
	using Exceptions;

	public class MessageTypeSubscription<TComponent, TMessage> : 
		IMessageTypeSubscription
		where TComponent : class, Consumes<TMessage>.All
		where TMessage : class
	{
		private readonly Type _messageType;
		private readonly Consumes<TMessage>.Selected _componentConsumer;

		public MessageTypeSubscription(SubscriptionMode mode, IObjectBuilder builder)
		{
			_messageType = typeof(TMessage);

			if (mode == SubscriptionMode.Selected)
				_componentConsumer = new SelectiveComponentDispatcher<TComponent, TMessage>(builder);
			else
				_componentConsumer = new ComponentDispatcher<TComponent, TMessage>(builder);
		}

		public void Subscribe<T>(MessageTypeDispatcher dispatcher, T component) where T : class
		{
			Consumes<TMessage>.All consumer = component as Consumes<TMessage>.All;
			if (consumer == null)
				throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof (T), _messageType));

			Subscribe(dispatcher, consumer);
		}

		public void Subscribe(MessageTypeDispatcher dispatcher, Consumes<TMessage>.All consumer)
		{
			IMessageDispatcher<TMessage> messageDispatcher = dispatcher.GetMessageProducer<TMessage>();

			messageDispatcher.Attach(consumer);
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
			IMessageDispatcher<TMessage> messageDispatcher = dispatcher.GetMessageProducer<TMessage>();

			messageDispatcher.Detach(consumer);
		}

		public void AddComponent(MessageTypeDispatcher dispatcher)
		{
			IMessageDispatcher<TMessage> messageDispatcher = dispatcher.GetMessageProducer<TMessage>();

			messageDispatcher.Attach(_componentConsumer);
		}

		public void RemoveComponent(MessageTypeDispatcher dispatcher)
		{
			IMessageDispatcher<TMessage> messageDispatcher = dispatcher.GetMessageProducer<TMessage>();

			messageDispatcher.Detach(_componentConsumer);
		}
	}
}