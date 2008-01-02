using System;
using System.Collections.Generic;

namespace MassTransit.ServiceBus
{
    public class MessageEndpoint<T> :
        IMessageEndpoint<T>,
        IMessageEndpointReceive where T : IMessage
    {
        private readonly IEndpoint _endpoint;
        private readonly List<MessageReceivedCallback<T>> _callbacks = new List<MessageReceivedCallback<T>>();

        public MessageEndpoint(IEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public void Send(IEnvelope e)
        {
            _endpoint.Send(e);
        }

        public event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public string Address
        {
            get { return _endpoint.Address; }
        }

        public void OnMessageReceived(IServiceBus bus, IEnvelope envelope, IMessage message)
        {
            MessageContext<T> context = new MessageContext<T>(bus, envelope, (T)message);

            _callbacks.ForEach(
                delegate(MessageReceivedCallback<T> callback)
                    {
                        try
                        {
                            callback(context);
                        }
                        catch (Exception ex)
                        {
                        }
                        
                    });
        }

        public IEndpoint Poison
        {
            get { throw new NotImplementedException(); }
        }


        public void Dispose()
        {
            this._endpoint.Dispose();
        }

        public void Subscribe(MessageReceivedCallback<T> callback)
        {
            _callbacks.Add(callback);
        }

        public void Subscribe(MessageReceivedCallback<T> callback, Predicate<T> condition)
        {
            _callbacks.Add(
                delegate(MessageContext<T> ctx)
                    {
                        if (condition(ctx.Message) == false)
                            return;

                        callback(ctx);
                    });
        }
    }
}