namespace MassTransit.Scheduling
{
    using System;


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
