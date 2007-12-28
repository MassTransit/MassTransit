using System;
using System.Collections.Generic;
using System.Messaging;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace MassTransit.ServiceBus.Subscriptions
{
    public class MsmqSubscriptionStorage :
        ISubscriptionStorage
    {
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private BinaryFormatter _formatter;
        private Cursor _peekCursor;
        private MessageQueue _storageQueue;
        private string _storageQueueName;
    	private readonly ISubscriptionStorage _subscriptionCache; // i like this
        private IEndpoint _defaultEndpoint;

        public MsmqSubscriptionStorage(string storageQueueName, IEndpoint defaultEndpoint, ISubscriptionStorage subscriptionCache) //what if this is the cache? maybe have another constructor?
        {
            _storageQueueName = storageQueueName;
            _defaultEndpoint = defaultEndpoint;
            _subscriptionCache = subscriptionCache;
            _storageQueue = new MessageQueue(_storageQueueName, QueueAccessMode.SendAndReceive);

            MessagePropertyFilter mpf = new MessagePropertyFilter();
            mpf.SetAll();

            _storageQueue.MessageReadPropertyFilter = mpf;

            _formatter = new BinaryFormatter();

            _peekCursor = _storageQueue.CreateCursor();

            _storageQueue.PeekCompleted += Queue_PeekCompleted;

            _storageQueue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Current, this, QueuePeekCompleted);
        }

        private void Queue_PeekCompleted(object sender, PeekCompletedEventArgs e)
        {
        }


        private void QueuePeekCompleted(IAsyncResult asyncResult)
        {
            Message msg = _storageQueue.EndPeek(asyncResult);

			_log.DebugFormat("Subscription Update Received: Id {0}", msg.Id);

			IMessage[] messages = _formatter.Deserialize(msg.BodyStream) as IMessage[];
            if (messages != null)
            {
                foreach (SubscriptionMessage subscriptionMessage in messages)
                {
					_log.DebugFormat("Subscription Endpoint: {0} Message Type: {1} Mode: {2}", msg.ResponseQueue.Path, subscriptionMessage.MessageType, subscriptionMessage.ChangeType.ToString());
                    if (subscriptionMessage.ChangeType == SubscriptionMessage.SubscriptionChangeType.Add) //would there ever be anything but?
                    {
                    	IEndpoint endpoint = (MessageQueueEndpoint)subscriptionMessage.Address;
						if(endpoint != null)
						{
							_subscriptionCache.Add(subscriptionMessage.MessageType, endpoint);
						}
                    }
                }
            }

            _storageQueue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Next, this, QueuePeekCompleted);
        }

        public IList<IEndpoint> List<T>(params T[] messages)
        {
            return _subscriptionCache.List(messages);
        }

        public void Add(Type messageType, IEndpoint endpoint)
        {
            SubscriptionMessage subscriptionMessage =
                new SubscriptionMessage(messageType, endpoint.Transport.Address,
                                        SubscriptionMessage.SubscriptionChangeType.Add);

            Send(subscriptionMessage);

            _subscriptionCache.Add(messageType, endpoint);
        }

        private void Send(IMessage message)
        {
            Message msg = new Message();

            msg.ResponseQueue = new MessageQueue(_defaultEndpoint.Transport.Address);
            msg.Recoverable = true;

            _formatter.Serialize(msg.BodyStream, new IMessage[] { message });

            _storageQueue.Send(msg);
        }

        public void Remove(Type messageType, IEndpoint endpoint)
        {
            _subscriptionCache.Remove(messageType, endpoint);

            SubscriptionMessage subscriptionMessage =
                 new SubscriptionMessage(messageType, endpoint.Transport.Address,
                                         SubscriptionMessage.SubscriptionChangeType.Remove);

            Send(subscriptionMessage);
        }

    }
}