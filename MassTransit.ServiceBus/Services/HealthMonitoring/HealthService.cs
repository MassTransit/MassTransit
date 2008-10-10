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
namespace MassTransit.ServiceBus.Services.HealthMonitoring
{
    public class HealthService :
        IHostedService
    {
        private readonly IServiceBus _bus;


        public HealthService(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
            _bus.AddComponent<HeartbeatMonitor>();
            _bus.AddComponent<Investigator>();
            _bus.AddComponent<Reporter>();
        }

        public void Stop()
        {
            _bus.RemoveComponent<Reporter>();
            _bus.RemoveComponent<Investigator>();
            _bus.RemoveComponent<HeartbeatMonitor>();
        }

        public void Dispose()
        {
            //nothing yet
        }
    }
}