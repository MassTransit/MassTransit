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

    	public void Send(IEnvelope e)
    	{
    		_endpoint.Send(e);
    	}

    	public event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived;

    	public string Address
        {
            get
            {
            	return _endpoint.Address;
            }
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