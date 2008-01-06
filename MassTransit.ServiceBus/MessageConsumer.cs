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

        public void Deliver(IServiceBus bus, IEnvelope envelope, IMessage message)
        {
            MessageContext<T> context = new MessageContext<T>(bus, envelope, (T) message);

            foreach (CallbackItem<T> item in _callbacks)
            {
                try
                {
                    if (item.Condition != null)
                    {
                        if (item.Condition(context.Message) == false)
                            break;
                    }

                    item.Callback(context);
                }
                catch (Exception ex)
                {
                    if (_log.IsDebugEnabled)
                        _log.Debug("Error in Callback", ex);
                }
            }
        }

        public bool MeetsCriteria(IMessage message)
        {
            foreach (CallbackItem<T> item in _callbacks)
            {
                if (item.Condition == null)
                    return true;

                try
                {
                    if (item.Condition((T)message))
                        return true;
                }
                catch (Exception ex)
                {
                    //TODO: why are we swallowing the exception
                    if(_log.IsWarnEnabled)
                        _log.Warn("Meets Criteria Issue", ex);
                }
            }

            return false;
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