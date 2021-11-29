namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System.Collections.Generic;
    using Topology;


    public class InvalidAmazonSqsConsumeTopologySpecification :
        IAmazonSqsConsumeTopologySpecification
    {
        readonly string _key;
        readonly string _message;

        public InvalidAmazonSqsConsumeTopologySpecification(string key, string message)
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
