namespace MassTransit.PrometheusIntegration
{
    using System;
    using System.Threading.Tasks;


    public class PrometheusBusObserver :
        IBusObserver
    {
        public void PostCreate(IBus bus)
        {
            bus.ConnectPublishObserver(new PrometheusPublishObserver());
            bus.ConnectSendObserver(new PrometheusSendObserver());
            bus.ConnectReceiveObserver(new PrometheusReceiveObserver());
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
            PrometheusMetrics.BusStarted();

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
            PrometheusMetrics.BusStopped();

            return Task.CompletedTask;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            PrometheusMetrics.BusStopped();

            return Task.CompletedTask;
        }
    }
}
