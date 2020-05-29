namespace MassTransit.Pipeline.Observables
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Util;


    public class BusObservable :
        Connectable<IBusObserver>,
        IBusObserver
    {
        public Task PostCreate(IBus bus)
        {
            return ForEachAsync(x => x.PostCreate(bus));
        }

        public Task CreateFaulted(Exception exception)
        {
            return ForEachAsync(x => x.CreateFaulted(exception));
        }

        public Task PreStart(IBus bus)
        {
            return ForEachAsync(x => x.PreStart(bus));
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return ForEachAsync(x => x.PostStart(bus, busReady));
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return ForEachAsync(x => x.StartFaulted(bus, exception));
        }

        public Task PreStop(IBus bus)
        {
            return ForEachAsync(x => x.PreStop(bus));
        }

        public Task PostStop(IBus bus)
        {
            return ForEachAsync(x => x.PostStop(bus));
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return ForEachAsync(x => x.StopFaulted(bus, exception));
        }
    }
}
