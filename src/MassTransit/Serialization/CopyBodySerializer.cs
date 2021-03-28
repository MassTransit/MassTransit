namespace MassTransit.Serialization
{
    using System.IO;
    using System.Net.Mime;


    /// <summary>
    /// Copies the body of the receive context to the send context unmodified
    /// </summary>
    public class CopyBodySerializer :
        IMessageSerializer
    {
        readonly ReceiveContext _context;

        public CopyBodySerializer(ReceiveContext context)
        {
            _context = context;
            ContentType = context.ContentType;
        }

        public ContentType ContentType { get; }

        void IMessageSerializer.Serialize<T>(Stream stream, SendContext<T> context)
        {
            using var bodyStream = _context.GetBodyStream();

            bodyStream.CopyTo(stream);
        }
    }
}
