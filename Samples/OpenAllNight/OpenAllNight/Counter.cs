namespace OpenAllNight
{
	using System.Threading;

	public class Counter
	{
		private long _messagesReceived = 0;
		private long _messagesSent = 0;
		private long _publishCount = 0;
		private bool _subscribed;

		public long PublishCount
		{
			get { return _publishCount; }
		}

		public long MessagesSent
		{
			get { return _messagesSent; }
		}

		public long MessagesReceived
		{
			get { return _messagesReceived; }
		}

		public bool Subscribed
		{
			get { return _subscribed; }
			set { _subscribed = value; }
		}

		public void IncrementMessagesSent()
		{
			Interlocked.Increment(ref _messagesSent);
		}

		public void IncrementMessagesReceived()
		{
			Interlocked.Increment(ref _messagesReceived);
		}

		public void IncrementPublishCount()
		{
			if(_subscribed)
				Interlocked.Increment(ref _publishCount);
		}
	}
}