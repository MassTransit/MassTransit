using System;

namespace Starbucks.Messages
{
    using MassTransit;

    [Serializable]
    public class PaymentDueMessage :
        CorrelatedBy<string>
    {
        public PaymentDueMessage(Guid starbucksTransactionId, string name, decimal amount)
        {
            StarbucksTransactionId = starbucksTransactionId;
            CorrelationId = name;
            Amount = amount;
        }

        public string Name
        {
            get { return CorrelationId; }
        }

        public Guid StarbucksTransactionId { get; private set;}

        public decimal Amount { get; private set; }
        public string CorrelationId { get; private set; }
    }
}