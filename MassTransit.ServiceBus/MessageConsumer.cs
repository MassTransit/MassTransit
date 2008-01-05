using System;
using System.Collections.Generic;
using log4net;

namespace MassTransit.ServiceBus
{
    public class MessageConsumer<T> :
        IMessageConsumer<T>,
        INotifyMessageConsumer where T : IMessage
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueEndpoint));

        private readonly List<CallbackItem<T>> _callbacks = new List<CallbackItem<T>>();

        #region IMessageConsumer<T> Members

        public void Subscribe(MessageReceivedCallback<T> callback)
        {
            _callbacks.Add(new CallbackItem<T>(callback));
        }

        public void Subscribe(MessageReceivedCallback<T> callback, Predicate<T> condition)
        {
            _callbacks.Add(new CallbackItem<T>(callback, condition));
        }

        #endregion

        #region INotifyMessageConsumer Members

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

                            if (context.Accept())
                            {
                                item.Callback(context);
                            }
                        }
                        catch (Exception ex)
                        {
                            if(_log.IsDebugEnabled)
                                _log.Debug("Error in Callback", ex);
                        }
                    });

            if (!context.WasAccepted)
            {
                if(_log.IsInfoEnabled)
                    _log.InfoFormat("Envelope Id {0} was not accepted", context.Envelope.Id);
            }
        }

        #endregion

        #region Nested type: CallbackItem

        internal class CallbackItem<T1> where T1 : IMessage
        {
            private MessageReceivedCallback<T1> _callback;
            private Predicate<T1> _condition;

            public CallbackItem(MessageReceivedCallback<T1> callback)
            {
                _callback = callback;
            }

            public CallbackItem(MessageReceivedCallback<T1> callback, Predicate<T1> condition)
            {
                _callback = callback;
                _condition = condition;
            }

            public MessageReceivedCallback<T1> Callback
            {
                get { return _callback; }
                set { _callback = value; }
            }

            public Predicate<T1> Condition
            {
                get { return _condition; }
                set { _condition = value; }
            }
        }

        #endregion
    }
}