namespace MassTransit.Scheduling
{
    using System;


    public class ScheduledRecurringMessageHandle<T> :
        ScheduledRecurringMessage<T>
        where T : class
    {
        public ScheduledRecurringMessageHandle(RecurringSchedule schedule, Uri destination, T payload)
        {
            Schedule = schedule;
            Destination = destination;
            Payload = payload;
        }

        public RecurringSchedule Schedule { get; private set; }
        public Uri Destination { get; private set; }
        public T Payload { get; private set; }
    }
}
