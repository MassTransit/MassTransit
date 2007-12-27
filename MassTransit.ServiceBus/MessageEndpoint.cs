using System;

namespace MassTransit.ServiceBus
{
    public class MessageEndpoint<T> : 
        IMessageEndpoint<T>,
		IMessageEndpointReceive where T: IMessage
    {
        private readonly IEndpoint _endpoint;

        public MessageEndpoint(IEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        public string Address
        {
            get { throw new NotImplementedException(); }
        }

        public ITransport Transport
        {
            get { return _endpoint.Transport; }
        }

    	public void OnMessageReceived(IEnvelope envelope, IMessage message)
    	{
			if (message is T)
			{
				if (MessageReceived != null)
				{
					MessageReceived(this, new MessageReceivedEventArgs<T>(envelope, (T) message));
				}
			}
    	}
    }
}