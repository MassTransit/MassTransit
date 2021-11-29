namespace MassTransit
{
    /// <summary>
    /// Published when a message fails to deserialize at the endpoint
    /// </summary>
    public interface ReceiveFault :
        Fault
    {
        /// <summary>
        /// The specified content type of the message by the transport
        /// </summary>
        string ContentType { get; }
    }
}
