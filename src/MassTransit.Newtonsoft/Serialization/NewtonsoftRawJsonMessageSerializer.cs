namespace MassTransit.Serialization
{
    using System.Net.Mime;
    using Newtonsoft.Json;


    public class NewtonsoftRawJsonMessageSerializer :
        RawMessageSerializer,
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/json";
        public static readonly ContentType RawJsonContentType = new ContentType(ContentTypeHeaderValue);

        readonly RawSerializerOptions _options;

        public NewtonsoftRawJsonMessageSerializer(RawSerializerOptions options = RawSerializerOptions.Default)
        {
            _options = options;
        }

        public static JsonSerializer Deserializer => NewtonsoftJsonMessageSerializer.Deserializer;

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            if (_options.HasFlag(RawSerializerOptions.AddTransportHeaders))
                SetRawMessageHeaders<T>(context);

            return new NewtonsoftRawJsonMessageBody<T>(context);
        }

        public ContentType ContentType => RawJsonContentType;
    }
}
