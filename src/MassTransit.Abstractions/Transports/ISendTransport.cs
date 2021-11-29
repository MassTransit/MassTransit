namespace MassTransit.Transports
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface ISendTransport :
        ISendObserverConnector
    {
        /// <summary>
        /// Send a message to the transport. The transport creates the OldSendContext, and calls back to
        /// allow the context to be modified to customize the message delivery.
        /// The transport specifies the defaults for the message as configured, and then allows the
        /// caller to modify the send context to include the required settings (durable, mandatory, etc.).
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message</param>
        /// <param name="pipe">The pipe invoked when sending a message, to do extra stuff</param>
        /// <param name="cancellationToken">Cancel the send operation (if possible)</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class;
    }
}
