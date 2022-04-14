namespace MassTransit.Tests.ReliableMessaging
{
    using System;


    public class CreateState
    {
        public Guid CorrelationId { get; set; }
        public bool FailOnFirstAttempt { get; set; }
        public bool FailMessageDelivery { get; set; }
    }
}
