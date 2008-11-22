namespace MassTransit.ServiceBus.Tests
{
	using Messages;

	public class ParticularConsumer :
		Consumes<PingMessage>.Selected
	{
		private readonly bool _accept;
		private PingMessage _consumed;

		public ParticularConsumer(bool accept)
		{
			_accept = accept;
		}

		public PingMessage Consumed
		{
			get { return _consumed; }
		}

		public void Consume(PingMessage message)
		{
			_consumed = message;
		}

		public bool Accept(PingMessage message)
		{
			return _accept;
		}
	}
}