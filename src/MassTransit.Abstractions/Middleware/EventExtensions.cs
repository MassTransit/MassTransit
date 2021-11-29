namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Middleware;


    public static class EventExtensions
    {
        public static Task PublishEvent<T>(this IPipe<EventContext> pipe, T message)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var context = new PublishEventContext<T>(message);

            return pipe.Send(context);
        }


        class PublishEventContext<T> :
            BasePipeContext,
            EventContext<T>
            where T : class
        {
            public PublishEventContext(T @event)
            {
                Event = @event;
                Timestamp = DateTime.UtcNow;
            }

            public DateTime Timestamp { get; }

            public T Event { get; }
        }
    }
}
