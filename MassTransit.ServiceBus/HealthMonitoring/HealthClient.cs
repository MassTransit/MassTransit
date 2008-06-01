namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;
    using Messages;

    public class HealthClient :
        IHostedService, 
        IDisposable //, Publishes<Heartbeat>
    {
        readonly IServiceBus _bus;
        readonly System.Timers.Timer _timer;

        public HealthClient(IServiceBus bus)
        {
            _bus = bus;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += Beat;
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
            _bus.Publish(new Heartbeat(3, _bus.Endpoint.Uri));
        }


        public void Dispose()
        {
            _timer.Elapsed -= Beat;
            _timer.Dispose();
        }
    }
}