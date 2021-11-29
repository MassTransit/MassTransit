namespace MassTransit.ActiveMqTransport
{
    using System.IO;
    using System.Text;
    using Apache.NMS;


    public class ActiveMqMessageBody :
        MessageBody
    {
        readonly IMessage _message;
        byte[] _bytes;

        public ActiveMqMessageBody(IMessage message)
        {
            _message = message;
        }

        public long? Length => _bytes?.Length;

        public Stream GetStream()
        {
            return new MemoryStream(GetBytes(), false);
        }

        public byte[] GetBytes()
        {
            return _bytes ??= _message switch
            {
                ITextMessage text => Encoding.UTF8.GetBytes(text.Text),
                IBytesMessage bytes => bytes.Content,
                _ => throw new ActiveMqTransportException($"The message type is not supported: {TypeCache.GetShortName(_message.GetType())}")
            };
        }

        public string GetString()
        {
            return _message switch
            {
                ITextMessage text => text.Text,
                IBytesMessage bytes => MessageDefaults.Encoding.GetString(bytes.Content),
                _ => throw new ActiveMqTransportException($"The message type is not supported: {TypeCache.GetShortName(_message.GetType())}")
            };
        }
    }
}
