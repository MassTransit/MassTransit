using System;
using Marten.Schema;
using MassTransit.Saga;

namespace MassTransit.MartenIntegration.Tests
{
    public class TestSaga : ISaga
    {
        [Identity]
        public Guid CorrelationId { get; set; }
        public Guid Id => CorrelationId;
    }
}