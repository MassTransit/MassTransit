namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;

    [Serializable]
    public class Ping :
        CorrelatedBy<Guid>
    {
        private readonly Guid _correlationId;

        public Ping(Guid correlationId)
        {
            _correlationId = correlationId;
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }
    }
}