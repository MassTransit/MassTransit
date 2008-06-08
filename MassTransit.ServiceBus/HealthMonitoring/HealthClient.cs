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
        IHostedService //, Publishes<Heartbeat>
    {
        private readonly IServiceBus _bus;
        private readonly Timer _timer;
        private readonly int _timeInSeconds;
        private readonly int _timeInMilliseconds;

        public HealthClient(IServiceBus bus) : this(bus, 3)
        {
        }
        public HealthClient(IServiceBus bus, int seconds)
        {
            _bus = bus;
            _timeInSeconds = seconds;
            _timeInMilliseconds = seconds*1000;
            _timer = new System.Timers.Timer(_timeInMilliseconds);
            _timer.Elapsed += Beat;
            _timer.AutoReset = true;
        }


        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Beat(object sender, System.Timers.ElapsedEventArgs e)
        {
            _bus.Publish(new Heartbeat(_timeInSeconds, _bus.Endpoint.Uri));
        }


        public void Dispose()
        {
            _timer.Elapsed -= Beat;
            _timer.Dispose();
        }


        public bool Enabled
        {
            get { return _timer.Enabled; }
        }
    }
}