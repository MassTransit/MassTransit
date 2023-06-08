namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using ProtoBuf.Meta;

    public class ProtobufMessageSerializer : IMessageSerializer
    {
        // might be application/vnd.google.protobuf or application/x-protobuf or application/x-google-protobuf or application/protobuf
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+pbuf";
        public static readonly ContentType ProtobufContentType = new ContentType(ContentTypeHeaderValue);
        private static RuntimeTypeModel _typeModel;

        public ContentType ContentType => ProtobufContentType;

        public ProtobufMessageSerializer(RuntimeTypeModel typeModel)
        {
            _typeModel = typeModel;
        }

        public MessageBody GetMessageBody<T>(SendContext<T> context) where T : class
        {
            return new ProtobufMessageBody<T>(context, _typeModel);
        }

    }
}
