namespace MassTransit.TestFramework.Sagas
{
    using System;


    public class TestInstance :
        SagaStateMachineInstance
    {
        public State CurrentState { get; set; }

        public string Key { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
