namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;

    [Serializable]
    public class Suspect
    {
        private Uri _endpointUri;


        public Suspect(Uri endpointUri)
        {
            _endpointUri = endpointUri;
        }


        public Uri EndpointUri
        {
            get { return _endpointUri; }
        }
    }
}