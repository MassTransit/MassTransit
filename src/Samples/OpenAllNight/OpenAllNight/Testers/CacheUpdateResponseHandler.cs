namespace OpenAllNight.Testers
{
	using log4net;
	using MassTransit;
	using MassTransit.Services.Subscriptions.Messages;

	public class CacheUpdateResponseHandler :
		Consumes<SubscriptionRefresh>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (CacheUpdateResponseHandler));
		private readonly Counter _counter;

		public CacheUpdateResponseHandler(Counter counter)
		{
			_counter = counter;
		}

		public void Consume(SubscriptionRefresh message)
		{
			_counter.IncrementMessagesReceived();
		}
	}
}