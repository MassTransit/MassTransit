namespace MassTransit.EventHubIntegration.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Registration;


    public class EventHubBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IRiderRegistrationContext _context;
        readonly IEventHubHostConfiguration _hostConfiguration;

        public EventHubBusInstanceSpecification(IRiderRegistrationContext context, IEventHubHostConfiguration hostConfiguration)
        {
            _context = context;
            _hostConfiguration = hostConfiguration;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _hostConfiguration.Validate();
        }

        public void Configure(IBusInstance busInstance)
        {
            var rider = _hostConfiguration.Build(_context, busInstance);
            busInstance.Connect<IEventHubRider>(rider);
        }
    }
}
