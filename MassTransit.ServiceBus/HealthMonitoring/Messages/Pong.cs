namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;

    [Serializable]
    public class Pong :
        CorrelatedBy<Guid>
    {
        private readonly Guid _correlationId;
        private readonly Uri _endpointUri;


        public Pong(Guid correlationId, Uri endpointUri)
        {
            _correlationId = correlationId;
            _endpointUri = endpointUri;
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        public Uri EndpointUri
        {
            get { return _endpointUri; }
        }
    }
}