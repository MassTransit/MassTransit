// Copyright 2007-2008 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//   http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Services.HealthMonitoring
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