using System;
using MassTransit.ServiceBus;

namespace Starbucks.Messages
{
    [Serializable]
    public class SubmitPaymentMessage : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; private set; }
        public PaymentType PaymentType { get; private set; }
        public decimal Amount { get; private set; }
        public string Name { get; private set; }

        public SubmitPaymentMessage(Guid correlationId, PaymentType paymentType, decimal amount, string name)
        {
            CorrelationId = correlationId;
            PaymentType = paymentType;
            Amount = amount;
            Name = name;
        }
    }
}