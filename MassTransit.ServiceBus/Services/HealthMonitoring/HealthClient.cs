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
    using System.Timers;
    using Messages;

    public class HealthClient :
        Consumes<Ping>.All,
        IHostedService //, Publishes<Heartbeat>
    {
        private readonly IServiceBus _bus;
        private readonly int _timeInMilliseconds;
        private readonly int _timeInSeconds;
        private readonly Timer _timer;

        public HealthClient(IServiceBus bus) : this(bus, 3)
        {
        }

        /// <summary>
        /// Constructs a new HealthClient object
        /// </summary>
        /// <param name="bus">The service bus to monitor</param>
        /// <param name="heartbeatInterval">The heartbeat interval in seconds</param>
        public HealthClient(IServiceBus bus, int heartbeatInterval)
        {
            _bus = bus;
            _timeInSeconds = heartbeatInterval;
            _timeInMilliseconds = heartbeatInterval*1000;
            _timer = new Timer(_timeInMilliseconds);
            _timer.Elapsed += Beat;
            _timer.AutoReset = true;
        }

        public bool Enabled
        {
            get { return _timer.Enabled; }
        }

        #region All Members

        public void Consume(Ping message)
        {
            _bus.Publish(new Pong(message.CorrelationId, _bus.Endpoint.Uri));
        }

        #endregion

        #region IHostedService Members

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Dispose()
        {
            _timer.Elapsed -= Beat;
            _timer.Dispose();
        }

        #endregion

        public void Beat(object sender, ElapsedEventArgs e)
        {
            _bus.Publish(new Heartbeat(_timeInSeconds, _bus.Endpoint.Uri));
        }
    }
}