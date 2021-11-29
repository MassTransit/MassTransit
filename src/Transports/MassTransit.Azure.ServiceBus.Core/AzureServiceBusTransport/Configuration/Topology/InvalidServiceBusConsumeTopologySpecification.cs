namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System.Collections.Generic;
    using Topology;


    public class InvalidServiceBusConsumeTopologySpecification :
        IServiceBusConsumeTopologySpecification
    {
        readonly string _key;
        readonly string _message;

        public InvalidServiceBusConsumeTopologySpecification(string key, string message)
        {
            _key = key;
            _message = message;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Failure(_key, _message);
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
        }
    }
}
