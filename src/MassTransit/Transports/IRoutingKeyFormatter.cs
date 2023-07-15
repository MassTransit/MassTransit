namespace MassTransit.Transports
{
    public interface IRoutingKeyFormatter
    {
        /// <summary>
        /// Format the routing key to be used by the transport, if supported
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message send context</param>
        /// <returns>The routing key to specify in the transport</returns>
        string FormatRoutingKey<T>(SendContext<T> context)
            where T : class;
    }
}
