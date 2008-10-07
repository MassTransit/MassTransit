namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;
    using System.Collections.Generic;
    using Messages;

    public class InMemoryHeartbeatTimer :
        IHeartbeatTimer
    {
        private readonly Dictionary<Uri, MonitorInfo> _monitoredEndpoints;
        private readonly IServiceBus _bus;

        public InMemoryHeartbeatTimer(IServiceBus bus)
        {
            _monitoredEndpoints = new Dictionary<Uri, MonitorInfo>();
            _bus = bus;
        }

        public void Add(Heartbeat message)
        {
            if (!_monitoredEndpoints.ContainsKey(message.EndpointAddress))
            {
                MonitorInfo info = new MonitorInfo(message.EndpointAddress,
                                                   message.TimeBetweenBeatsInSeconds, OnMissingHeartbeat);

                _monitoredEndpoints.Add(message.EndpointAddress, info);
            }
        }

        public void Remove(Heartbeat message)
        {
            if(_monitoredEndpoints.ContainsKey(message.EndpointAddress))
            {
                _monitoredEndpoints[message.EndpointAddress].Stop();
                _monitoredEndpoints.Remove(message.EndpointAddress);
            }
        }

        public void OnMissingHeartbeat(MonitorInfo info)
        {
            _bus.Publish(new Suspect(info.EndpointUri));

        }
    }
}