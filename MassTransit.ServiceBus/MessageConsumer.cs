using System;
using System.Collections.Generic;
using log4net;
using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// A message consumer is created when a service subscribes to a specific type of message
    /// on a service bus. 
    /// </summary>
    /// <typeparam name="T">The message type handled by this message consumer</typeparam>
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
                                                        "There was an exception in the MessageConsumer.IsHandled",
                                                        ex);
                }
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// Used to track a subscription to a message type on a service bus
    /// </summary>
    /// <typeparam name="T1">The type of message being handled</typeparam>
    public class MessageConsumerCallbackItem<T1> where T1 : IMessage
    {
        private MessageReceivedCallback<T1> _callback;
        private Predicate<T1> _condition;

        /// <summary>
        /// Initializes an instance of a <c ref="MessageConsumerCallbackItem" />
        /// </summary>
        /// <param name="callback">The callback method to handle the message</param>
        public MessageConsumerCallbackItem(MessageReceivedCallback<T1> callback)
        {
            _callback = callback;
        }

        /// <summary>
        /// Initializes an instance of a <c ref="MessageConsumerCallbackItem" />
        /// </summary>
        /// <param name="callback">The callback method to handle the message</param>
        /// <param name="condition">The predicate used to check if the callback will handle the message</param>
        public MessageConsumerCallbackItem(MessageReceivedCallback<T1> callback, Predicate<T1> condition)
        {
            _callback = callback;
            _condition = condition;
        }

        /// <summary>
        /// The callback method
        /// </summary>
        public MessageReceivedCallback<T1> Callback
        {
            get { return _callback; }
            set { _callback = value; }
        }

        /// <summary>
        /// The message filter condition
        /// </summary>
        public Predicate<T1> Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }
    }
}