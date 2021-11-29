namespace MassTransit
{
    using System;


    public static class SchedulingExtensions
    {
        public static Guid? GetSchedulingTokenId(this ConsumeContext context)
        {
            return context.Headers.Get(MessageHeaders.SchedulingTokenId, default(Guid?));
        }

        public static DateTimeOffset? GetQuartzScheduled(this ConsumeContext context)
        {
            return context.Headers.Get(MessageHeaders.Quartz.Scheduled, default(DateTimeOffset?));
        }

        public static DateTimeOffset? GetQuartzSent(this ConsumeContext context)
        {
            return context.Headers.Get(MessageHeaders.Quartz.Sent, default(DateTimeOffset?));
        }

        public static DateTimeOffset? GetQuartzNextScheduled(this ConsumeContext context)
        {
            return context.Headers.Get(MessageHeaders.Quartz.NextScheduled, default(DateTimeOffset?));
        }

        public static DateTimeOffset? GetQuartzPreviousSent(this ConsumeContext context)
        {
            return context.Headers.Get(MessageHeaders.Quartz.PreviousSent, default(DateTimeOffset?));
        }
    }
}
