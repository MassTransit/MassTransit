namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// MessageData is used when a property size may be larger than what should be sent via the message
    /// transport. This would includes attachments such as images, documents, videos, etc. Using MessageData,
    /// it is possible to include large properties without sending them in the actual message. The claim check
    /// pattern is the common reference.
    /// </summary>
    /// <typeparam name="T">
    /// The type used to access the message data, valid types include stream, string, and byte[].
    /// </typeparam>
    public interface MessageData<T> :
        IMessageData
    {
        /// <summary>
        /// The property value, which may be loaded asynchronously from the message data repository.
        /// </summary>
        Task<T> Value { get; }
    }
}
