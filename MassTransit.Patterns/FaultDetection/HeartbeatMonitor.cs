namespace MassTransit.Patterns.FaultDetection
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus;
    using MassTransit.Patterns.FaultDetection.Messages;

    public class HeartbeatMonitor /*:
        IConsume<Heartbeat>*/
    {
        private Dictionary<IEndpoint, MonitorInfo> _monitoredEndpoints;
        private IServiceBus _bus;

        public HeartbeatMonitor(IServiceBus bus)
        {
            _bus = bus;
            _monitoredEndpoints = new Dictionary<IEndpoint, MonitorInfo>();
        }

        public void Handle(IMessageContext<Heartbeat> ctx)
        {
            if (!_monitoredEndpoints.ContainsKey(ctx.Envelope.ReturnEndpoint))
            {
                _monitoredEndpoints.Add(ctx.Envelope.ReturnEndpoint, new MonitorInfo(ctx.Envelope.ReturnEndpoint, DateTime.Now));
            }
        }
    }

    
}
