using System;

namespace MassTransit.ServiceBus
{
	public class MessageContext<T> :
		EventArgs where T : IMessage
	{
		private IEnvelope _envelope;
		private T _message;
	    private IServiceBus _bus;

		public MessageContext(IEnvelope envelope, T message, IServiceBus bus)
		{
			_envelope = envelope;
		    _bus = bus;
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

	    public IServiceBus Bus
	    {
	        get { return _bus; }
	        set { _bus = value; }
	    }

	    public void Reply(params IMessage[] messages)
	    {
	        Bus.Send(Envelope.ReturnTo, messages);
	    }
	}
}