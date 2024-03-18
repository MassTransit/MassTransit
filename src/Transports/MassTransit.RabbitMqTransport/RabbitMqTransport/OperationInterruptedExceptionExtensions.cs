namespace MassTransit.RabbitMqTransport
{
    using RabbitMQ.Client.Exceptions;


    public static class OperationInterruptedExceptionExtensions
    {
        public static bool ChannelShouldBeClosed(this OperationInterruptedException ex)
        {
            return ex.ShutdownReason?.ReplyCode switch
            {
                403 => true, // access refused
                404 => true, // not found
                405 => true, // locked
                406 => true, // precondition failed
                491 => true, // the channel was already closed (MT-internal)
                _ => false
            };
        }
    }
}
