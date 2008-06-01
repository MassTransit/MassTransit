namespace MassTransit.Patterns.FaultDetection
{
	using System.Collections.Generic;
	using Messages;
	using ServiceBus;

	public class HeartbeatMonitor :
		Consumes<Heartbeat>.All
	{
		private IServiceBus _bus;
		private Dictionary<IEndpoint, MonitorInfo> _monitoredEndpoints;

		public HeartbeatMonitor(IServiceBus bus)
		{
			_bus = bus;
			_monitoredEndpoints = new Dictionary<IEndpoint, MonitorInfo>();
		}

		public void Consume(Heartbeat message)
		{
/*
			if (!_monitoredEndpoints.ContainsKey(ctx.Message.Pulse))
			{
				_monitoredEndpoints.Add(ctx.Envelope.ReturnEndpoint, new MonitorInfo(ctx.Envelope.ReturnEndpoint, DateTime.Now));
			}
*/
		}
	}
}