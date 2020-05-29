namespace MassTransit.Scheduling
{
    using System;


    public class CancelScheduledMessageCommand :
        CancelScheduledMessage
    {
        public CancelScheduledMessageCommand(Guid tokenId)
        {
            CorrelationId = NewId.NextGuid();
            Timestamp = DateTime.UtcNow;

            TokenId = tokenId;
        }

        public Guid TokenId { get; }
        public DateTime Timestamp { get; }
        public Guid CorrelationId { get; }
    }
}
