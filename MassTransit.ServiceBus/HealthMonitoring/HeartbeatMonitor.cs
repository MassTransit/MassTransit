/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;
    using System.Collections.Generic;
    using Messages;

    public class HeartbeatMonitor :
        Consumes<Heartbeat>.All //, Produces<Suspect>
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

        public bool AmIWatchingYou(Uri uri)
        {
            return _monitoredEndpoints.ContainsKey(uri);
        }

        public void OnMissingHeartbeat(MonitorInfo info)
        {
            _bus.Publish(new Suspect(info.EndpointUri));

        }
    }
}