using System.Collections.Generic;
using GreenPipes;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public class EventStoreDbBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IRiderRegistrationContext _context;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;

        public EventStoreDbBusInstanceSpecification(IRiderRegistrationContext context, IEventStoreDbHostConfiguration hostConfiguration)
        {
            _context = context;
            _hostConfiguration = hostConfiguration;
        }

        public IEnumerable<ValidationResult> Validate() => _hostConfiguration.Validate();

        public void Configure(IBusInstance busInstance)
        {
            var rider = _hostConfiguration.Build(_context, busInstance);
            busInstance.Connect<IEventStoreDbRider>(rider);
        }
    }
}
