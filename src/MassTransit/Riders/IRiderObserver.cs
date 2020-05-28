namespace MassTransit.Riders
{
    using System;
    using System.Threading.Tasks;


    public interface IRiderObserver
    {
        Task StartFaulted(Exception exception);
        Task PreStart(IRider rider);
        Task PostStart(IRider rider);

        Task StopFaulted(Exception exception);
        Task PreStop(IRider rider);
        Task PostStop(IRider rider);
    }
}
