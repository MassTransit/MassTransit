namespace MassTransit.Riders
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class RiderBusObserver :
        IBusObserver
    {
        readonly IRider _rider;

        public RiderBusObserver(IRider rider)
        {
            _rider = rider;
        }

        public Task PostCreate(IBus bus)
        {
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
            return _rider.Start();
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStop(IBus bus)
        {
            return _rider.Stop();
        }

        public Task PostStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}
