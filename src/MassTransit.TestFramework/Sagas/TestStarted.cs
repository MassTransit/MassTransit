namespace MassTransit.TestFramework.Sagas
{
    using System;


    public class TestStarted :
        CorrelatedBy<Guid>
    {
        public string TestKey { get; set; }
        public Guid CorrelationId { get; set; }
    }
}