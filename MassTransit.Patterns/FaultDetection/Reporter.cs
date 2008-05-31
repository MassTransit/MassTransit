namespace MassTransit.Patterns.FaultDetection
{
	using log4net;
	using Messages;
	using ServiceBus;

	public class Reporter : Consumes<DownEndpoint>.Any
	{
		private readonly ILog _log = LogManager.GetLogger(typeof (Reporter));

		public void Consume(DownEndpoint message)
		{
			//	_log.ErrorFormat("Endpoint '{0}' is down", cxt.Envelope.ReturnEndpoint);
		}
	}
}