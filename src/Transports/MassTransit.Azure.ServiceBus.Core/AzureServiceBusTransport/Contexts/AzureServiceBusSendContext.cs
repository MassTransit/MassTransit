namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using Context;


    static class ScheduledMessageToken
    {
        internal static readonly ulong Tag;
        internal static readonly byte[] Key;

        static ScheduledMessageToken()
        {
            var guid = new Guid("E25FC12B-FF28-4476-A6E1-DE45E154A675");

            Key = guid.ToByteArray();

            Tag = BitConverter.ToUInt64(Key, 8);
        }
    }


    public class AzureServiceBusSendContext<T> :
        MessageSendContext<T>,
        ServiceBusSendContext<T>
        where T : class
    {
        string _partitionKey;
        string _sessionId;

        public AzureServiceBusSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
        }

        public override TimeSpan? Delay
        {
            get => ScheduledEnqueueTimeUtc.HasValue ? ScheduledEnqueueTimeUtc.Value - DateTime.UtcNow : default;
            set => ScheduledEnqueueTimeUtc = value > TimeSpan.Zero ? DateTime.UtcNow + value.Value : default(DateTime?);
        }

        public string ReplyToSessionId { get; set; }

        public DateTime? ScheduledEnqueueTimeUtc { get; set; }

        public string PartitionKey
        {
            get => _partitionKey;
            set
            {
                _partitionKey = value;

                if (string.IsNullOrWhiteSpace(_sessionId) || _sessionId.Equals(value))
                    return;

                _sessionId = null;
            }
        }

        public string SessionId
        {
            get => _sessionId;
            set
            {
                _partitionKey = value;
                _sessionId = value;
            }
        }

        public void SetScheduledMessageId(long sequenceNumber)
        {
            var key = ScheduledMessageToken.Key;

            var bytes = new byte[16];
            Buffer.BlockCopy(key, 8, bytes, 8, 8);

            var sequenceNumberBytes = BitConverter.GetBytes(sequenceNumber);

            var sequenceLength = sequenceNumberBytes.Length;

            Buffer.BlockCopy(sequenceNumberBytes, 0, bytes, 0, sequenceLength);

            ScheduledMessageId = new Guid(bytes);
        }

        public bool TryGetScheduledMessageId(out long sequenceNumber)
        {
            if (ScheduledMessageId.HasValue)
                return TryGetSequenceNumber(ScheduledMessageId.Value, out sequenceNumber);

            sequenceNumber = 0;
            return false;
        }

        public bool TryGetSequenceNumber(Guid id, out long sequenceNumber)
        {
            var bytes = id.ToByteArray();

            if (BitConverter.ToUInt64(bytes, 8) == ScheduledMessageToken.Tag)
            {
                sequenceNumber = BitConverter.ToInt64(bytes, 0);
                return true;
            }

            sequenceNumber = default;
            return false;
        }

        public long GetSequenceNumber(Guid scheduledMessageId)
        {
            return BitConverter.ToInt64(scheduledMessageId.ToByteArray(), 0);
        }
    }
}
