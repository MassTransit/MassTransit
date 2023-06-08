namespace MassTransit.Serialization
{
    using MassTransit.Events;
    using ProtoBuf.Meta;
    using System.Net.Mime;
    public class ProtobufSerializerFactory<TProtoMessage> : ISerializerFactory
        where TProtoMessage : class
    {
        private readonly RuntimeTypeModel _typeModel;

        public ContentType ContentType => ProtobufMessageSerializer.ProtobufContentType;

        public ProtobufSerializerFactory()
        {
            _typeModel = RuntimeTypeModel.Create();

            _typeModel.UseImplicitZeroDefaults = false;
            _typeModel.AutoAddMissingTypes = true;
            _typeModel.AllowParseableTypes = true;
            _typeModel.AutoCompile = true;

            _typeModel.Add<ReceiveFaultEvent>(true);
            var receiveFaultModel = _typeModel.Add<ReceiveFault>(true);
            receiveFaultModel.AddSubType(100, typeof(ReceiveFaultEvent));
        }

        public IMessageSerializer CreateSerializer()
        {
            return new ProtobufMessageSerializer(_typeModel);
        }
        public IMessageDeserializer CreateDeserializer()
        {
            return new ProtobufMessageDeserializer<TProtoMessage>(_typeModel);
        }
    }
}
