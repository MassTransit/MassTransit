namespace MassTransit.Pipeline.Observables
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Util;
    using Riders;


    public class RiderObservable :
        Connectable<IRiderObserver>,
        IRiderObserver
    {
        public Task StartFaulted(Exception exception)
        {
            return ForEachAsync(x => x.StartFaulted(exception));
        }

        public Task PreStart(IRider rider)
        {
            return ForEachAsync(x => x.PreStart(rider));
        }

        public Task PostStart(IRider rider)
        {
            return ForEachAsync(x => x.PostStart(rider));
        }

        public Task StopFaulted(Exception exception)
        {
            return ForEachAsync(x => x.StopFaulted(exception));
        }

        public Task PreStop(IRider rider)
        {
            return ForEachAsync(x => x.PreStop(rider));
        }

        public Task PostStop(IRider rider)
        {
            return ForEachAsync(x => x.PostStop(rider));
        }
    }
}
