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
    using System.Timers;

    public class MonitorInfo
    {
        public Uri EndpointAddress;
        private readonly Timer _timer;
        private readonly int _millisecondsInASecond = 1000;
        private readonly OnMissingHeartbeatDelegate _dlg;

        public MonitorInfo(Uri endpointAddress, int timeBetweenBeatsInSeconds, OnMissingHeartbeatDelegate dlg)
        {
            _dlg = dlg;
            EndpointAddress = endpointAddress;
            _timer = new Timer(timeBetweenBeatsInSeconds/_millisecondsInASecond);
            _timer.AutoReset = false;
            _timer.Start();
            _timer.Elapsed += OnElapse;
        }

        private void OnElapse(object sender, ElapsedEventArgs e)
        {
            _dlg(this);
        }

        public delegate void OnMissingHeartbeatDelegate(MonitorInfo info);

        public void Reset()
        {
            _timer.Stop();
            _timer.Start();
        }
    }
}