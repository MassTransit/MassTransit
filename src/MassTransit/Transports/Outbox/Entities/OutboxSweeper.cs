using System;

namespace MassTransit.Transports.Outbox.Entities
{
    public class OutboxSweeper
    {
        public string OutboxName { get; set; }
        public string InstanceId { get; set; }
        public DateTime LastCheckinTime { get; set; }
        public TimeSpan CheckinInterval { get; set; }
    }
}
