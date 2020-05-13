namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Hangfire.Common;
    using Hangfire.Server;
    using Newtonsoft.Json;


    class SchedulerJobDataAttribute :
        JobFilterAttribute,
        IServerFilter
    {
        public void OnPerforming(PerformingContext filterContext)
        {
            var data = filterContext.BackgroundJob.Job.Args.OfType<HangfireScheduledMessageData>().Single();
            var timeHeaders = new Dictionary<string, DateTimeOffset?>
            {
                [HangfireMessageHeaders.Sent] = DateTimeOffset.UtcNow,
                [HangfireMessageHeaders.Scheduled] = filterContext.BackgroundJob.CreatedAt,
            };
            data.PayloadMessageHeadersAsJson = JsonConvert.SerializeObject(timeHeaders);
        }

        public void OnPerformed(PerformedContext filterContext)
        {
        }
    }
}
