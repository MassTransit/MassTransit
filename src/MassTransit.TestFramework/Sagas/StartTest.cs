namespace MassTransit.TestFramework.Sagas
{
    using System;


    public class StartTest :
        CorrelatedBy<Guid>
    {
        public string TestKey { get; set; }
        public Guid CorrelationId { get; set; }
    }
}