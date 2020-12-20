namespace MassTransit.KafkaIntegration.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;


    public class KafkaBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IKafkaHostConfiguration _hostConfiguration;

        public KafkaBusInstanceSpecification(IKafkaHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public void Configure(IBusInstance busInstance)
        {
            var rider = _hostConfiguration.Build(busInstance);
            busInstance.Connect<IKafkaRider>(rider);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _hostConfiguration.Validate();
        }
    }
}
