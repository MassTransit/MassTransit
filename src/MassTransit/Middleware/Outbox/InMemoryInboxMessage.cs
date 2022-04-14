namespace MassTransit.Middleware.Outbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public class InMemoryInboxMessage
    {
        readonly SemaphoreSlim _inUse;

        readonly List<InMemoryOutboxMessage> _outboxMessages;

        public InMemoryInboxMessage(Guid messageId, Guid consumerId)
        {
            MessageId = messageId;
            ConsumerId = consumerId;

            _inUse = new SemaphoreSlim(1);
            _outboxMessages = new List<InMemoryOutboxMessage>();
        }

        /// <summary>
        /// The MessageId of the incoming message
        /// </summary>
        public Guid MessageId { get; }

        /// <summary>
        /// And MD5 hash of the endpoint name + consumer type
        /// </summary>
        public Guid ConsumerId { get; }

        /// <summary>
        /// When the message was first received
        /// </summary>
        public DateTime Received { get; set; }

        /// <summary>
        /// How many times the message has been received
        /// </summary>
        public int ReceiveCount { get; set; }

        /// <summary>
        /// If present, when the message expires (from the message header)
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// When the message was consumed, successfully
        /// </summary>
        public DateTime? Consumed { get; set; }

        /// <summary>
        /// When all messages in the outbox were delivered to the transport
        /// </summary>
        public DateTime? Delivered { get; set; }

        /// <summary>
        /// The last sequence number that was successfully delivered to the transport
        /// </summary>
        public long? LastSequenceNumber { get; set; }

        public Task MarkInUse(CancellationToken cancellationToken)
        {
            return _inUse.WaitAsync(cancellationToken);
        }

        public void Release()
        {
            _inUse.Release();
        }

        public List<InMemoryOutboxMessage> GetOutboxMessages()
        {
            lock (_outboxMessages)
                return _outboxMessages.ToList();
        }

        public void RemoveOutboxMessages()
        {
            lock (_outboxMessages)
                _outboxMessages.Clear();
        }

        public void AddOutboxMessage(InMemoryOutboxMessage outboxMessage)
        {
            lock (_outboxMessages)
            {
                outboxMessage.SequenceNumber = _outboxMessages.Count + 1;

                _outboxMessages.Add(outboxMessage);
            }
        }
    }
}
