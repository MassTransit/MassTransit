using System;
using System.Collections.Generic;

namespace MassTransit.ServiceBus
{
    public class MessageEndpoint<T> :
        IMessageEndpoint<T>,
        IMessageEndpointReceive where T : IMessage
    {
        internal class CallbackItem<T> where T : IMessage
        {
            private MessageReceivedCallback<T> _callback;
            private Predicate<T> _condition;

            public MessageReceivedCallback<T> Callback
            {
                get { return _callback; }
                set { _callback = value; }
            }

            public Predicate<T> Condition
            {
                get { return _condition; }
                set { _condition = value; }
            }

            public CallbackItem(MessageReceivedCallback<T> callback)
            {
                _callback = callback;
            }

            public CallbackItem(MessageReceivedCallback<T> callback, Predicate<T> condition)
            {
                _callback = callback;
                _condition = condition;
            }
        }

        private readonly IEndpoint _endpoint;
        private readonly List<CallbackItem<T>> _callbacks = new List<CallbackItem<T>>();

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
            MessageContext<T> context = new MessageContext<T>(bus, envelope, (T) message);

            _callbacks.ForEach(
                delegate(CallbackItem<T> item)
                    {
                        try
                        {
                            if (item.Condition != null)
                            {
                                if (item.Condition(context.Message) == false)
                                    return;
                            }

                            item.Callback(context);
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
            _callbacks.Clear();
        }

        public void Subscribe(MessageReceivedCallback<T> callback)
        {
            _callbacks.Add(new CallbackItem<T>(callback));
        }

        public void Subscribe(MessageReceivedCallback<T> callback, Predicate<T> condition)
        {
            _callbacks.Add(new CallbackItem<T>(callback, condition));
        }
    }
}