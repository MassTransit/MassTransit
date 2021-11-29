namespace MassTransit.HangfireIntegration
{
    using System.Collections.Generic;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.Server;


    public interface IHangfireComponentResolver
    {
        IBackgroundJobClient BackgroundJobClient { get; }
        IRecurringJobManager RecurringJobManager { get; }
        ITimeZoneResolver TimeZoneResolver { get; }
        IJobFilterProvider JobFilterProvider { get; }
        IEnumerable<IBackgroundProcess> BackgroundProcesses { get; }
        JobStorage JobStorage { get; }
    }
}
