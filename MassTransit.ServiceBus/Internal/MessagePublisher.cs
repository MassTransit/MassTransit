namespace MassTransit.ServiceBus.Internal
{
	using System;
	using Exceptions;

	public class MessagePublisher<TMessage> : 
		IProducerTypeInfo
		where TMessage : class
	{
		private readonly Type _messageType;
		private ProducesPublisher<TMessage> _publisher;

		public MessagePublisher(IServiceBus bus)
		{
			_messageType = typeof(TMessage);

			_publisher = new ProducesPublisher<TMessage>(bus);
		}

		public void Attach<T>(T component) where T : class
		{
			Produces<TMessage> producer = component as Produces<TMessage>;
			if (producer == null)
				throw new ConventionException(string.Format("Object of type {0} does not produce messages of type {1}", typeof(T), _messageType));

			Attach(producer);
		}

		public void Attach(Produces<TMessage> producer)
		{
			producer.Attach(_publisher);
		}

		public void Dispose()
		{
			_publisher = null;
		}
	}
}