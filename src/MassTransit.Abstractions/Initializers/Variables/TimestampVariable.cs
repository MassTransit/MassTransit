namespace MassTransit.Initializers.Variables
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Used to set timestamp(s) in a message, which is the same regardless of how many times it is
    /// used within the same initialize message context
    /// </summary>
    public class TimestampVariable :
        IInitializerVariable<DateTime>
    {
        readonly DateTime _timestamp;

        public TimestampVariable()
        {
            _timestamp = DateTime.UtcNow;
        }

        public TimestampVariable(DateTime timestamp)
        {
            _timestamp = timestamp;
        }

        Task<DateTime> IInitializerVariable<DateTime>.GetValue<TMessage>(InitializeContext<TMessage> context)
        {
            var timestampContext = context.GetOrAddPayload<TimestampContext>(() => new Context(_timestamp));

            return Task.FromResult(timestampContext.Timestamp);
        }

        public static implicit operator DateTime(TimestampVariable variable)
        {
            return variable._timestamp;
        }


        interface TimestampContext
        {
            DateTime Timestamp { get; }
        }


        class Context :
            TimestampContext
        {
            public Context(DateTime timestamp)
            {
                Timestamp = timestamp;
            }

            public DateTime Timestamp { get; }
        }
    }
}
