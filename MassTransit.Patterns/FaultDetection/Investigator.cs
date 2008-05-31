namespace MassTransit.Patterns.FaultDetection
{
	using Messages;
	using ServiceBus;

	public class Investigator : Consumes<Suspect>.Any
	{
		private readonly IServiceBus _bus;

		public Investigator(IServiceBus bus)
		{
			_bus = bus;
		}

		public void Consume(Suspect message)
		{
//			IServiceBusAsyncResult async = _bus.Request<Ping>(cxt.Message.Endpoint, new Ping());
//			async.AsyncWaitHandle.WaitOne(3000, true);
//			IList<IMessage> msg = async.Messages;
//
//			if (msg.Count > 0)
//			{
//				//I have an endpoint in a weird state
//			}
//			else
//			{
//				//I have confirmed dead endpoint
//				_bus.Publish<DownEndpoint>(new DownEndpoint());
//			}
		}
	}
}