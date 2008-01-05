using System;
using System.Collections.Generic;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace MassTransit.ServiceBus.Subscriptions
{
	public class MsmqSubscriptionStorage :
		ISubscriptionStorage
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (MsmqSubscriptionStorage));

		private BinaryFormatter _formatter;
		private Cursor _peekCursor;
		private MessageQueue _storageQueue;
		private string _storageQueueName;
		private readonly ISubscriptionStorage _subscriptionCache;
		private IEndpoint _endpoint;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageEndpoint">the name of the queue that stores all of the subscriptions</param>
        /// <param name="endpoint"></param>
        /// <param name="subscriptionCache">in memory cache</param>
		public MsmqSubscriptionStorage(IEndpoint storageEndpoint, IEndpoint endpoint, ISubscriptionStorage subscriptionCache)
		{
            _storageQueueName = storageEndpoint.Address;
			_endpoint = endpoint;
			_subscriptionCache = subscriptionCache;
			_storageQueue = new MessageQueue(_storageQueueName, QueueAccessMode.SendAndReceive);

			Initialize();
		}

		private void Initialize()
		{
			MessagePropertyFilter mpf = new MessagePropertyFilter();
			mpf.SetAll();

			_storageQueue.MessageReadPropertyFilter = mpf;

			_formatter = new BinaryFormatter();

			_peekCursor = _storageQueue.CreateCursor();

			_storageQueue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Current, this, QueuePeekCompleted);
		}


		private void QueuePeekCompleted(IAsyncResult asyncResult)
		{
			if (_storageQueue == null)
				return;

			Message msg = _storageQueue.EndPeek(asyncResult);

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Subscription Update Received: Id {0}", msg.Id);

			IMessage[] messages = _formatter.Deserialize(msg.BodyStream) as IMessage[];
			if (messages != null)
			{
				foreach (SubscriptionMessage subscriptionMessage in messages)
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("Subscription Subscribe: {0} Message Type: {1} Mode: {2}", msg.ResponseQueue.Path, subscriptionMessage.MessageType, subscriptionMessage.ChangeType.ToString());

					if (subscriptionMessage.ChangeType == SubscriptionMessage.SubscriptionChangeType.Add) //would there ever be anything but?
					{
						IEndpoint endpoint = (MessageQueueEndpoint) subscriptionMessage.Address;
						if (endpoint != null)
						{
							_subscriptionCache.Add(subscriptionMessage.MessageType, endpoint);
						}
					}
				}
			}

			_storageQueue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Next, this, QueuePeekCompleted);
		}

		public IList<IEndpoint> List<T>(params T[] messages) where T : IMessage
		{
			return _subscriptionCache.List(messages);
		}

		public void Add(Type messageType, IEndpoint endpoint)
		{
			SubscriptionMessage subscriptionMessage =
				new SubscriptionMessage(messageType, endpoint.Address,
				                        SubscriptionMessage.SubscriptionChangeType.Add);

			Send(subscriptionMessage);

			_subscriptionCache.Add(messageType, endpoint);

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Adding Subscription to {0} for {1}", messageType, endpoint.Address);
		}

		private void Send(IMessage message)
		{
			Message msg = new Message();

			msg.ResponseQueue = new MessageQueue(_endpoint.Address);
			msg.Recoverable = true;

			_formatter.Serialize(msg.BodyStream, new IMessage[] {message});

			_storageQueue.Send(msg);
		}

		public void Remove(Type messageType, IEndpoint endpoint)
		{
			_subscriptionCache.Remove(messageType, endpoint);

			SubscriptionMessage subscriptionMessage =
				new SubscriptionMessage(messageType, endpoint.Address,
				                        SubscriptionMessage.SubscriptionChangeType.Remove);

			Send(subscriptionMessage);

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Removing Subscription to {0} for {1}", messageType, endpoint.Address);
		}

	    public void Dispose()
	    {
	        _storageQueue.Close();
	        _storageQueue.Dispose();
            _storageQueue = null;

	        _subscriptionCache.Dispose();
	    }
	}
}