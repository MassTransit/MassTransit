namespace MassTransit.Audit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Used to store message audits that are observed
    /// </summary>
    public interface IMessageAuditStore
    {
        /// <summary>
        /// Store the message audit, with associated metadata
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message itself</param>
        /// <param name="metadata">The message metadata</param>
        /// <returns></returns>
        Task StoreMessage<T>(T message, MessageAuditMetadata metadata)
            where T : class;
    }
}
