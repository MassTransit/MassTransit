namespace MassTransit.Monitoring
{
    using System;
    using System.Threading.Tasks;


    public class InstrumentBusObserver :
        IBusObserver
    {
        public void PostCreate(IBus bus)
        {
            bus.ConnectPublishObserver(new InstrumentPublishObserver());
            bus.ConnectSendObserver(new InstrumentSendObserver());
            bus.ConnectReceiveObserver(new InstrumentReceiveObserver());
        }

        public void CreateFaulted(Exception exception)
        {
        }

        public Task PreStart(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return Task.CompletedTask;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task PreStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task PostStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
