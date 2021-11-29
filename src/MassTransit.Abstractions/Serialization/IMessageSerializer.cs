#nullable enable
namespace MassTransit
{
    using System.Net.Mime;


    /// <summary>
    /// A message serializer is responsible for serializing a message. Shocking, I know.
    /// </summary>
    public interface IMessageSerializer
    {
        ContentType ContentType { get; }

        /// <summary>
        /// Returns a message body, for the serializer, which can be used by the transport to obtain the
        /// serialized message in the desired format.
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class;
    }
}
