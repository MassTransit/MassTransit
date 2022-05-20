namespace MassTransit.InMemoryTransport.Configuration
{
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using Transports.Fabric;


    public class InvalidInMemoryConsumeTopologySpecification :
        IInMemoryConsumeTopologySpecification
    {
        readonly string _key;
        readonly string _message;

        public InvalidInMemoryConsumeTopologySpecification(string key, string message)
        {
            _key = key;
            _message = message;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Failure(_key, _message);
        }

        public void Apply(IMessageFabricConsumeTopologyBuilder builder)
        {
        }
    }
}
