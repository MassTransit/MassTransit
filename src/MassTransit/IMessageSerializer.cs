namespace MassTransit
{
    using System.IO;
    using System.Net.Mime;


    /// <summary>
    /// A message serializer is responsible for serializing a message. Shocking, I know.
    /// </summary>
    public interface IMessageSerializer
    {
        ContentType ContentType { get; }

        /// <summary>
        /// Serialize the message to the stream provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="context"></param>
        void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class;
    }
}
