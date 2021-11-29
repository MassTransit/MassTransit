namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public class ConnectReceiveEndpointObserverBusObserver<T> :
        IBusObserver
        where T : class, IReceiveEndpointObserver
    {
        readonly IServiceProvider _provider;

        public ConnectReceiveEndpointObserverBusObserver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void PostCreate(IBus bus)
        {
            var observer = _provider.GetService<T>();
            if (observer != null)
                bus.ConnectReceiveEndpointObserver(observer);
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
