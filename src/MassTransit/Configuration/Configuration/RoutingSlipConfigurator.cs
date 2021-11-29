namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Courier.Contracts;


    public class RoutingSlipConfigurator :
        IRoutingSlipConfigurator,
        IBuildPipeConfigurator<ConsumeContext<RoutingSlip>>
    {
        readonly IBuildPipeConfigurator<ConsumeContext<RoutingSlip>> _configurator;

        public RoutingSlipConfigurator()
        {
            _configurator = new PipeConfigurator<ConsumeContext<RoutingSlip>>();
        }

        public IPipe<ConsumeContext<RoutingSlip>> Build()
        {
            return _configurator.Build();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurator.Validate();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<RoutingSlip>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }
    }
}
