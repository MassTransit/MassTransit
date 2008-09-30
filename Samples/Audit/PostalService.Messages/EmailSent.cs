namespace PostalService.Messages
{
    using System;
    using MassTransit.ServiceBus;

    [Serializable]
    public class EmailSent :
        CorrelatedBy<Guid>
    {
        public EmailSent(Guid correlationId)
        {
            CorrelationId = correlationId;

            SentAt = DateTime.UtcNow;
        }

        public DateTime SentAt { get; private set; }
        public Guid CorrelationId { get; private set; }
    }
}