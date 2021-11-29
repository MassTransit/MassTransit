namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class RawXmlMessageSerializer :
        RawMessageSerializer,
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/xml";
        public static readonly ContentType RawXmlContentType = new ContentType(ContentTypeHeaderValue);

        readonly RawSerializerOptions _options;

        public RawXmlMessageSerializer(RawSerializerOptions options = RawSerializerOptions.Default)
        {
            _options = options;
        }

        public ContentType ContentType => RawXmlContentType;

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            if (_options.HasFlag(RawSerializerOptions.AddTransportHeaders))
                SetRawMessageHeaders<T>(context);

            return new NewtonsoftRawXmlMessageBody<T>(context);
        }
    }
}
