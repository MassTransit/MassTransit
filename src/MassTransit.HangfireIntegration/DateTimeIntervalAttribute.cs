namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Linq;
    using Hangfire;
    using Hangfire.Client;
    using Hangfire.Common;
    using Hangfire.Server;


    class DateIntervalAttribute : JobFilterAttribute,
        IClientFilter,
        IServerFilter
    {
        public void OnCreating(CreatingContext filterContext)
        {
            var now = DateTimeOffset.Now;
            DateTimeOffset notBefore = filterContext.Job.Args.OfType<DateTimeOffset>().First();
            DateTimeOffset? notAfter = filterContext.Job.Args.OfType<DateTimeOffset?>().LastOrDefault();

            if (notBefore > now)
                filterContext.Canceled = true;
            else if (notAfter.HasValue && notAfter.Value < now)
            {
                filterContext.Canceled = true;
                if (!filterContext.Parameters.TryGetValue("RecurringJobId", out var recurringJobId))
                    return;
                RecurringJob.RemoveIfExists((string)recurringJobId);
            }
        }

        public void OnCreated(CreatedContext filterContext)
        {
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            var now = DateTimeOffset.Now;
            DateTimeOffset notBefore = filterContext.BackgroundJob.Job.Args.OfType<DateTimeOffset>().First();
            DateTimeOffset? notAfter = filterContext.BackgroundJob.Job.Args.OfType<DateTimeOffset?>().LastOrDefault();

            if (notBefore > now)
                filterContext.Canceled = true;
            else if (notAfter.HasValue && notAfter.Value < now)
                filterContext.Canceled = true;
        }

        public void OnPerformed(PerformedContext filterContext)
        {
        }
    }
}
