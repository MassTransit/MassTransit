namespace MassTransit.Transports
{
    using System.IO;
    using Internal;
    using log4net;
    using Serialization;

    public class MulticastUdpMessageSelector :
        IMessageSelector
    {
        private static readonly ILog _messageLog = SpecialLoggers.Messages;

        private readonly MulticastUdpEndpoint _endpoint;
        private readonly byte[] _data;
        private readonly IMessageSerializer _serializer;
        private bool _accepted;
        private object _message;

        public MulticastUdpMessageSelector(MulticastUdpEndpoint endpoint, byte[] data, IMessageSerializer serializer)
        {
            _endpoint = endpoint;
            _data = data;
            _serializer = serializer;
        }

        public void Dispose()
        {
            if (_accepted) return;

            if (_messageLog.IsInfoEnabled)
                _messageLog.InfoFormat("SKIP:{0}:{1}", _endpoint.Uri, _message != null ? _message.GetType().Name : "(Unknown)");
        }

        public bool AcceptMessage()
        {
            if (_messageLog.IsInfoEnabled)
                _messageLog.InfoFormat("RECV:{0}:{1}", _endpoint.Uri, _message != null ? _message.GetType().Name : "(Unknown)");

            _accepted = true;

            // we were able to get the message without conflict, because we don't support peeking with udp
            return true;
        }

        public void MoveMessageTo(IEndpoint endpoint)
        {
            throw new System.NotImplementedException();
        }

        public object DeserializeMessage()
        {
            using (MemoryStream mstream = new MemoryStream(_data))
            {
                _message = _serializer.Deserialize(mstream);

                return _message;
            }
        }
    }
}