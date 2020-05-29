namespace MassTransit.MartenIntegration.Tests
{
    using System;
    using Marten.Schema;
    using MassTransit.Saga;


    public class TestSaga : ISaga
    {
        public Guid Id => CorrelationId;

        [Identity]
        public Guid CorrelationId { get; set; }
    }
}
