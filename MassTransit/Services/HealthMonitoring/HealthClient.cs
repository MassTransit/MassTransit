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
    using System.Timers;
    using Internal;
    using Magnum;
    using Messages;

    public class HealthClient :
        Consumes<Ping>.All,
        IBusService
    {
        private IServiceBus _bus;
        private readonly int _timeInMilliseconds;
        private readonly int _timeInSeconds;
        private readonly Timer _timer;
        private readonly Guid _systemId;

        public HealthClient() : this(3)
        {
        }

        /// <summary>
        /// Constructs a new HealthClient object
        /// </summary>
        /// <param name="heartbeatInterval">The heartbeat interval in seconds</param>
        public HealthClient(int heartbeatInterval)
        {
            _timeInSeconds = heartbeatInterval;
            _timeInMilliseconds = heartbeatInterval*1000;
            _timer = new Timer(_timeInMilliseconds);
            _timer.Elapsed += Beat;
            _timer.AutoReset = true;
            _systemId = CombGuid.Generate();
        }

        public Guid SystemId
        {
            get { return _systemId; }
        }

        public bool Enabled
        {
            get { return _timer.Enabled; }
        }

        public void Consume(Ping message)
        {
            _bus.Publish(new Pong(message.CorrelationId, _bus.Endpoint.Uri));
        }

        public void Start(IServiceBus bus)
        {
            _bus = bus;
            _timer.Start();
            _bus.ControlBus.Publish(new EndpointTurningOn(_bus.Endpoint.Uri, _timeInSeconds, _systemId));
        }

        public void Stop()
        {
			_bus.ControlBus.Publish(new EndpointTurningOff(_systemId));
            _timer.Stop();
        }

        public void Dispose()
        {
            _timer.Elapsed -= Beat;
            _timer.Dispose();
        }

        public void Beat(object sender, ElapsedEventArgs e)
        {
			_bus.ControlBus.Publish(new Heartbeat(_bus.Endpoint.Uri, _systemId));
        }
    }
}