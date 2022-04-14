namespace MassTransit.Tests.ReliableMessaging
{
    using System;


    public class ReliableState :
        SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
