namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;

    [Serializable]
    public class Ping :
        CorrelatedBy<Guid>, IEquatable<Ping>
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


        public bool Equals(Ping ping)
        {
            if (ping == null) return false;
            return Equals(_correlationId, ping._correlationId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Ping);
        }

        public override int GetHashCode()
        {
            return _correlationId.GetHashCode();
        }
    }
}