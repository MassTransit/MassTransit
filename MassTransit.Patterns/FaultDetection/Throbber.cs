namespace MassTransit.Patterns.FaultDetection
{
    using System;
    using MassTransit.ServiceBus;
    using MassTransit.Patterns.FaultDetection.Messages;

    public class Throbber :
        IDisposable
    {
        IServiceBus _bus;
        System.Timers.Timer _timer;

        public Throbber(IServiceBus bus)
        {
            _bus = bus;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += Beat;
        }

        public void Beat(object sender, System.Timers.ElapsedEventArgs e)
        {
            _bus.Publish<Heartbeat>(new Heartbeat());
        }


        public void Dispose()
        {
        }
    }
}
