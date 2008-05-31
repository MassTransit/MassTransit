namespace MassTransit.ServiceBus.Internal
{
	using System;
	using log4net;

	/// <summary>
	/// A message context contains the participants in a message exchange that is
	/// received on the service bus.
	/// </summary>
	/// <typeparam name="T">The message type</typeparam>
	public class MessageContext<T> :
		EventArgs, IMessageContext<T> where T : class
	{
		private readonly IServiceBus _bus;
		private readonly IEnvelope _envelope;
		private readonly ILog _log = LogManager.GetLogger(typeof (MessageContext<T>));
		private readonly T _message;

		/// <summary>
		/// Initializes an instance of the <c ref="MessageContext" /> class
		/// </summary>
		/// <param name="bus">The service bus on which the message was received</param>
		/// <param name="envelope">The message envelope received</param>
		/// <param name="message">The individual message from the envelope being received</param>
		public MessageContext(IServiceBus bus, IEnvelope envelope, T message)
		{
			_envelope = envelope;
			_bus = bus;
			_message = message;
		}

		/// <summary>
		/// The envelope containing the message
		/// </summary>
		public IEnvelope Envelope
		{
			get { return _envelope; }
		}

		/// <summary>
		/// The actual message being delivered
		/// </summary>
		public T Message
		{
			get { return _message; }
		}

		/// <summary>
		/// The service bus on which the message was received
		/// </summary>
		public IServiceBus Bus
		{
			get { return _bus; }
		}

		/// <summary>
		/// Builds an envelope with the correlation id set to the id of the incoming envelope
		/// </summary>
		/// <param name="message">The messages to include with the reply</param>
		public void Reply(object message)
		{
			Bus.Publish(message);
		}

		/// <summary>
		/// Moves the specified messages to the back of the list of available 
		/// messages so they can be handled later. Could screw up message order.
		/// </summary>
		public void HandleMessageLater(object message)
		{
			Bus.Endpoint.Send(message);
		}

		/// <summary>
		/// Marks the whole context as poison
		/// </summary>
		public void MarkPoison()
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Envelope {0} Was Marked Poisonous", _envelope.Id);

			//Bus.PoisonEndpoint.Send(_envelope);
			// TODO
		}

		/// <summary>
		/// Marks a specific message as poison
		/// </summary>
		public void MarkPoison(object message)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("A Message in Envelope {0} Was Marked Poisonous", _envelope.Id);

			//Bus.PoisonEndpoint.Send(env);
			//TODO
		}
	}
}