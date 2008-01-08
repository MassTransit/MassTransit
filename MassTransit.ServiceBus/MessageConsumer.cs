using System;
using System.Collections.Generic;
using log4net;
using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    public class MessageConsumer<T> :
        IMessageConsumer<T>,
        INotifyMessageConsumer where T : IMessage
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueEndpoint));

        private readonly List<MessageConsumerCallbackItem<T>> _callbacks = new List<MessageConsumerCallbackItem<T>>();

        #region IMessageConsumer<T> Members

        public void Subscribe(MessageReceivedCallback<T> callback)
        {
            _callbacks.Add(new MessageConsumerCallbackItem<T>(callback));
        }

        public void Subscribe(MessageReceivedCallback<T> callback, Predicate<T> condition)
        {
            _callbacks.Add(new MessageConsumerCallbackItem<T>(callback, condition));
        }

        #endregion

        #region INotifyMessageConsumer Members

        public void Deliver(IServiceBus bus, IEnvelope envelope, IMessage message)
        {
            MessageContext<T> context = new MessageContext<T>(bus, envelope, (T) message);

            foreach (MessageConsumerCallbackItem<T> item in _callbacks)
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
            foreach (MessageConsumerCallbackItem<T> item in _callbacks)
            {
                if (item.Condition == null)
                    return true;

                try
                {
                    if (item.Condition((T) message))
                        return true;
                }
                catch (Exception ex)
                {
                    throw new MeetsCriteriaException<T>(item,
                                                        "There was an exception in the MessageConsumer.MeetsCriteria",
                                                        ex);
                }
            }

            return false;
        }

        #endregion
    }

    public class MessageConsumerCallbackItem<T1> where T1 : IMessage
    {
        private readonly MessageReceivedCallback<T1> _callback;
        private readonly Predicate<T1> _condition;

        public MessageConsumerCallbackItem(MessageReceivedCallback<T1> callback)
        {
            _callback = callback;
        }

        public MessageConsumerCallbackItem(MessageReceivedCallback<T1> callback, Predicate<T1> condition)
        {
            _callback = callback;
            _condition = condition;
        }

        public MessageReceivedCallback<T1> Callback
        {
            get { return _callback; }
        }

        public Predicate<T1> Condition
        {
            get { return _condition; }
        }
    }
}