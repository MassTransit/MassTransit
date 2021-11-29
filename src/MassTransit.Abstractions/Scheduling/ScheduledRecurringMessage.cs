namespace MassTransit
{
    using System;
    using Scheduling;


    public interface ScheduledRecurringMessage
    {
        RecurringSchedule Schedule { get; }
        Uri Destination { get; }
    }


    public interface ScheduledRecurringMessage<out T> :
        ScheduledRecurringMessage
        where T : class
    {
        T Payload { get; }
    }
}
