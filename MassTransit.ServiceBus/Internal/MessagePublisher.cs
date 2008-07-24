namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections;
	using Exceptions;

	public class MessagePublisher<TMessage> :
		IProducerTypeInfo
		where TMessage : class
	{
		private readonly Type _messageType;
		private ProducesPublisher<TMessage> _publisher;

		public MessagePublisher(IServiceBus bus)
		{
			_messageType = typeof (TMessage);

			_publisher = new ProducesPublisher<TMessage>(bus);
		}

		public void Attach<T>(T component) where T : class
		{
			Produces<TMessage>.Bound producer = component as Produces<TMessage>.Bound;
			if (producer == null)
				throw new ConventionException(string.Format("Object of type {0} does not produce messages of type {1}", typeof (T), _messageType));

			Attach(producer);
		}

		public Hashtable GetPublishers()
		{
			Hashtable ht = new Hashtable();

			string className = _messageType.Name;
			string argumentName = className.Substring(0, 1).ToLowerInvariant() + className.Substring(1) + "Consumer";

			ht.Add(argumentName, _publisher);

			return ht;
		}

		public void Dispose()
		{
			_publisher = null;
		}

		public void Attach(Produces<TMessage>.Bound producer)
		{
			producer.Attach(_publisher);
		}
	}
}