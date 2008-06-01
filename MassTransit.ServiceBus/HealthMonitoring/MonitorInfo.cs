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