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

        public event MessageHandler<T> MessageReceived;

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

    	public void OnMessageReceived(IServiceBus bus, IEnvelope envelope, IMessage message)
    	{
			if (message is T)
			{
			    MessageHandler<T> local = MessageReceived;
				if (local != null)
				{
					local(bus, envelope, (T) message);
				}
			}
    	}
    }
}