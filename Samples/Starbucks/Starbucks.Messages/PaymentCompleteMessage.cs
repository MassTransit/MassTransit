using System;
using MassTransit.ServiceBus;

namespace Starbucks.Messages
{
    [Serializable]
    public class PaymentCompleteMessage : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; private set; }
        public string Name { get; set; }
        public decimal Amount { get; private set; }

        public PaymentCompleteMessage(Guid correlationId, string name, decimal amount)
        {
            CorrelationId = correlationId;
            Name = name;
            Amount = amount;
        }
    }
}