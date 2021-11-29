#nullable enable
namespace MassTransit
{
    using System.IO;


    public interface MessageBody
    {
        long? Length { get; }

        /// <summary>
        /// Return the message body as a stream
        /// </summary>
        /// <returns></returns>
        Stream GetStream();

        /// <summary>
        /// Return the message body as a byte array
        /// </summary>
        /// <returns></returns>
        byte[] GetBytes();

        /// <summary>
        /// Return the message body as a string
        /// </summary>
        /// <returns></returns>
        string GetString();
    }
}
