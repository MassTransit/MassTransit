namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;

    public class HealthInformation
    {
        private readonly Uri _uri;
        private readonly int _secondsBetweenBeats;
        private readonly DateTime? _firstDetectedAt;
        private DateTime? _lastDetectedAt;
        private DateTime? _lastFaultDetectedAt;

        public HealthInformation(Uri uri, int secondsBetweenBeats)
        {
            _uri = uri;
            _secondsBetweenBeats = secondsBetweenBeats;
            _firstDetectedAt = DateTime.Now;
            _lastFaultDetectedAt = FirstDetectedAt;
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public int SecondsBetweenBeats
        {
            get { return _secondsBetweenBeats; }
        }

        public DateTime? FirstDetectedAt
        {
            get { return _firstDetectedAt; }
        }

        public DateTime? LastDetectedAt
        {
            get { return _lastDetectedAt; }
            set { _lastDetectedAt = value; }
        }

        public DateTime? LastFaultDetectedAt
        {
            get { return _lastFaultDetectedAt; }
            set { _lastFaultDetectedAt = value; }
        }
    }
}