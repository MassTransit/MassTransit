namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Used to reschedule delivery of the current message
    /// </summary>
    public interface MessageRedeliveryContext
    {
        /// <summary>
        /// Schedule the message to be redelivered after the specified delay with given operation.
        /// </summary>
        /// <param name="delay">The minimum delay before the message will be redelivered to the queue</param>
        /// <param name="callback">Operation which perform during message redeliver to queue</param>
        /// <returns></returns>
        Task ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext>? callback = null);
    }
}
