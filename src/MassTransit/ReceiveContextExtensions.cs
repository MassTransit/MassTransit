namespace MassTransit
{
    using System;


    public static class ReceiveContextExtensions
    {
        /// <summary>
        /// Returns the messageId from the transport header, if available
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Guid? GetMessageId(this ReceiveContext context)
        {
            return context.TransportHeaders.Get<Guid>(MessageHeaders.MessageId);
        }
    }
}
