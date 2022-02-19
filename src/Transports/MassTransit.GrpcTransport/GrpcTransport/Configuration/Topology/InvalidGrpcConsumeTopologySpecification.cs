namespace MassTransit.GrpcTransport.Configuration
{
    using System.Collections.Generic;
    using MassTransit.Configuration;


    public class InvalidGrpcConsumeTopologySpecification :
        IGrpcConsumeTopologySpecification
    {
        readonly string _key;
        readonly string _message;

        public InvalidGrpcConsumeTopologySpecification(string key, string message)
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
