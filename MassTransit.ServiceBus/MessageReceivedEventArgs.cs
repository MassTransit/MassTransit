using System;

namespace MassTransit.ServiceBus
{
	public class MessageReceivedEventArgs<T> :
		EventArgs where T : IMessage
	{
		private IEnvelope _envelope;

		private T _message;

		public MessageReceivedEventArgs(IEnvelope envelope, T message)
		{
			_envelope = envelope;
			_message = message;
		}

		public IEnvelope Envelope
		{
			get { return _envelope; }
			set { _envelope = value; }
		}

		public T Message
		{
			get { return _message; }
			set { _message = value; }
		}
	}
}