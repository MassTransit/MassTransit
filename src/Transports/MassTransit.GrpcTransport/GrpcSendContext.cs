namespace MassTransit
{
    public interface GrpcSendContext<out T> :
        SendContext<T>,
        GrpcSendContext
        where T : class
    {
    }


    public interface GrpcSendContext :
        SendContext,
        RoutingKeySendContext
    {
        /// <summary>
        /// Specify that the published message must be delivered to a queue or it will be returned
        /// </summary>
        bool Mandatory { get; set; }

        /// <summary>
        /// The destination exchange for the message
        /// </summary>
        string Exchange { get; }
    }
}
