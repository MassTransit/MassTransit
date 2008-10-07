namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;

    [Serializable]
    public class HealthStatusRequest :
        CorrelatedBy<Guid>
    {
        private readonly Uri _requestingUri;
        private readonly Guid _correlationId;

        public HealthStatusRequest(Uri requestingUri, Guid correlationId)
        {
            _requestingUri = requestingUri;
            _correlationId = correlationId;
        }

        public Uri RequestingUri
        {
            get { return _requestingUri; }
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

    }
}