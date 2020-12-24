namespace MassTransit.EventHubIntegration.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Registration;


    public class EventHubBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IEventHubHostConfiguration _hostConfiguration;

        public EventHubBusInstanceSpecification(IEventHubHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _hostConfiguration.Validate();
        }

        public void Configure(IBusInstance busInstance)
        {
            var rider = _hostConfiguration.Build(busInstance);
            busInstance.Connect<IEventHubRider>(rider);
        }
    }
}
