namespace MassTransit.KafkaIntegration.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;


    public class KafkaBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IRiderRegistrationContext _context;
        readonly IKafkaHostConfiguration _hostConfiguration;

        public KafkaBusInstanceSpecification(IRiderRegistrationContext context, IKafkaHostConfiguration hostConfiguration)
        {
            _context = context;
            _hostConfiguration = hostConfiguration;
        }

        public void Configure(IBusInstance busInstance)
        {
            var rider = _hostConfiguration.Build(_context, busInstance);
            busInstance.Connect<IKafkaRider>(rider);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _hostConfiguration.Validate();
        }
    }
}
