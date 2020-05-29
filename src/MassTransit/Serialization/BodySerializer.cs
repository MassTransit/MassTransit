namespace MassTransit.Serialization
{
    using System.IO;
    using System.Net.Mime;


    /// <summary>
    /// A body serializer takes a byte array message body and just streams it out to the message
    /// unmodified.
    /// </summary>
    public class BodySerializer :
        IMessageSerializer
    {
        readonly byte[] _body;

        public BodySerializer(ContentType contentType, byte[] body)
        {
            ContentType = contentType;
            _body = body;
        }

        public ContentType ContentType { get; }

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            stream.Write(_body, 0, _body.Length);
        }
    }
}
