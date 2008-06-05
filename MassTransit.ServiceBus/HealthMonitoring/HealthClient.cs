namespace MassTransit.ServiceBus.HealthMonitoring
{
    using Messages;

    public class HealthClient :
        IHostedService //, Publishes<Heartbeat>
    {
        private readonly IServiceBus _bus;
        private readonly System.Timers.Timer _timer;
        private readonly int _timeInSeconds = 3;
        private readonly int _timeInMilliseconds = 3*1000;

        public HealthClient(IServiceBus bus)
        {
            _bus = bus;
            _timer = new System.Timers.Timer(_timeInMilliseconds);
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
            _bus.Publish(new Heartbeat(_timeInSeconds, _bus.Endpoint.Uri));
        }


        public void Dispose()
        {
            _timer.Elapsed -= Beat;
            _timer.Dispose();
        }
    }
}