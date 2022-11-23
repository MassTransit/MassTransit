namespace MassTransit.Scheduling
{
    using System;


    public class CancelScheduledMessageCommand :
        CancelScheduledMessage
    {
        public CancelScheduledMessageCommand()
        {
        }

        public CancelScheduledMessageCommand(Guid tokenId)
        {
            CorrelationId = NewId.NextGuid();
            Timestamp = DateTime.UtcNow;
            TokenId = tokenId;
        }

        public Guid CorrelationId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid TokenId { get; set; }
    }
}
