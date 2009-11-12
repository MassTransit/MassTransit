using System;
using MassTransit;

namespace DistributedGrid.Shared.Messages
{
    public class CompletedSimpleWorkItem :
        CorrelatedBy<Guid>
    {
        public CompletedSimpleWorkItem(Guid correlationId, DateTime requestCreatedAt)
        {
            CorrelationId = correlationId;
            CreatedAt = DateTime.UtcNow;
            RequestCreatedAt = requestCreatedAt;
        }

        protected CompletedSimpleWorkItem()
        {
        }

        public Guid CorrelationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RequestCreatedAt { get; set; }
    }
}