namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;
    using System.Collections.Generic;
    using Messages;

    public class HeartbeatMonitor :
        Consumes<Heartbeat>.All //, Publishes<Suspect>
    {
        private readonly IServiceBus _bus;
        private readonly Dictionary<Uri, MonitorInfo> _monitoredEndpoints;

        public HeartbeatMonitor(IServiceBus bus)
        {
            _bus = bus;
            _monitoredEndpoints = new Dictionary<Uri, MonitorInfo>();
        }

        public void Consume(Heartbeat message)
        {
			AddToWatch(message);

            _monitoredEndpoints[message.EndpointAddress].Reset();
        }

        public void AddToWatch(Heartbeat message)
        {
            if (!_monitoredEndpoints.ContainsKey(message.EndpointAddress))
            {
                MonitorInfo info = new MonitorInfo(message.EndpointAddress,
                    message.TimeBetweenBeatsInSeconds, OnMissingHeartbeat);

                _monitoredEndpoints.Add(message.EndpointAddress, info);
            }
        }

        public void OnMissingHeartbeat(MonitorInfo info)
        {
            _bus.Publish(new Suspect());
        }
    }
}