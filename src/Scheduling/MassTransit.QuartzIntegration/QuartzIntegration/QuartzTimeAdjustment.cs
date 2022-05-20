namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;


    public class QuartzTimeAdjustment :
        IDisposable
    {
        readonly ISchedulerFactory _schedulerFactory;
        TimeSpan _timeOffset = TimeSpan.Zero;

        public QuartzTimeAdjustment(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
            SystemTime.UtcNow = GetUtcNow;
            SystemTime.Now = GetNow;
        }

        public QuartzTimeAdjustment(IServiceProvider provider)
        {
            _schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
            SystemTime.UtcNow = GetUtcNow;
            SystemTime.Now = GetNow;
        }

        public void Dispose()
        {
            SystemTime.UtcNow = () => DateTimeOffset.UtcNow;
            SystemTime.Now = () => DateTimeOffset.Now;
        }

        public async Task AdvanceTime(TimeSpan duration)
        {
            var scheduler = await _schedulerFactory.GetScheduler().ConfigureAwait(false);

            await scheduler.Standby().ConfigureAwait(false);

            _timeOffset += duration;

            await scheduler.Start().ConfigureAwait(false);
        }

        DateTimeOffset GetUtcNow()
        {
            return DateTimeOffset.UtcNow + _timeOffset;
        }

        DateTimeOffset GetNow()
        {
            return DateTimeOffset.Now + _timeOffset;
        }
    }
}
