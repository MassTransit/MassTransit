namespace MassTransit.RabbitMqTransport
{
    using RabbitMQ.Client.Exceptions;


    public static class OperationInterruptedExceptionExtensions
    {
        public static bool ChannelShouldBeClosed(this OperationInterruptedException ex)
        {
            if (ex.ShutdownReason == null)
                return false;

            return ex.ShutdownReason?.ReplyCode >= 400;
        }
    }
}
