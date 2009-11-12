using System;
using MassTransit;

namespace DistributedGrid.Shared.Messages
{
    public class DoSimpleWorkItem :
        CorrelatedBy<Guid>
    {
        public DoSimpleWorkItem(Guid correlationId)
        {
            CorrelationId = correlationId;
            CreatedAt = DateTime.UtcNow;
        }

        protected DoSimpleWorkItem()
        {
        }

        public Guid CorrelationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}