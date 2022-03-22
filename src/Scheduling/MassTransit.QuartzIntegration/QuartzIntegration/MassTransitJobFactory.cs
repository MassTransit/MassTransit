namespace MassTransit.QuartzIntegration
{
    using System;
    using Quartz;
    using Quartz.Spi;


    public class MassTransitJobFactory :
        IJobFactory
    {
        readonly IBus _bus;

        public MassTransitJobFactory(IBus bus)
        {
            _bus = bus;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return new ScheduledMessageJob(_bus);
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
