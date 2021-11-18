namespace MassTransit.Pipeline.Observables
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Util;


    public class BusObservable :
        Connectable<IBusObserver>,
        IBusObserver
    {
        public void PostCreate(IBus bus)
        {
            All(x =>
            {
                x.PostCreate(bus);
                return true;
            });
        }

        public void CreateFaulted(Exception exception)
        {
            All(x =>
            {
                x.CreateFaulted(exception);
                return true;
            });
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
