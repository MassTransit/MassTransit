namespace RedisSaga
{
    using System;
    using MassTransit;
    using MassTransit.Saga;
    using Automatonymous;

    public class OrderState :
        SagaStateMachineInstance,
        ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }

        public DateTime? OrderDate { get; set; }

        public int Version { get; set; }
    }
}