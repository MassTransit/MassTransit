namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;

    public class HealthInformation
    {
        private readonly Uri _uri;
        private readonly DateTime? _firstDetectedAt;
        private DateTime? _lastFaultDetectedAt;

        public HealthInformation(Uri uri)
        {
            _uri = uri;
            _firstDetectedAt = DateTime.Now;
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public DateTime? FirstDetectedAt
        {
            get { return _firstDetectedAt; }
        }

        public DateTime? LastFaultDetectedAt
        {
            get { return _lastFaultDetectedAt; }
            set { _lastFaultDetectedAt = value; }
        }
    }
}