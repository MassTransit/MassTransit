namespace MassTransit.Configurators
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Registration;
    using Util;


    public class ConnectReceiveEndpointObserverBusObserver<T> :
        IBusObserver
        where T : class, IReceiveEndpointObserver
    {
        readonly IConfigurationServiceProvider _provider;
        ConnectHandle _handle;

        public ConnectReceiveEndpointObserverBusObserver(IConfigurationServiceProvider provider)
        {
            _provider = provider;
        }

        public Task PostCreate(IBus bus)
        {
            var observer = _provider.GetService<T>();
            if (observer != null)
                _handle = bus.ConnectReceiveEndpointObserver(observer);

            return TaskUtil.Completed;
        }

        public Task CreateFaulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStart(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return TaskUtil.Completed;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task PostStop(IBus bus)
        {
            _handle?.Disconnect();
            _handle = null;

            return TaskUtil.Completed;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}
