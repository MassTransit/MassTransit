namespace MassTransit.Configuration
{
    using ConsumePipeSpecifications;
    using Topology;
    using Topology.Observers;


    public class ConsumePipeConfiguration :
        IConsumePipeConfiguration
    {
        readonly ConsumePipeSpecification _specification;

        public ConsumePipeConfiguration(IConsumeTopology consumeTopology)
        {
            _specification = new ConsumePipeSpecification();
            _specification.ConnectConsumePipeSpecificationObserver(new TopologyConsumePipeSpecificationObserver(consumeTopology));
        }

        public ConsumePipeConfiguration(IConsumePipeSpecification parentSpecification)
        {
            _specification = new ConsumePipeSpecification();
            _specification.ConnectConsumePipeSpecificationObserver(new ParentConsumePipeSpecificationObserver(parentSpecification));
        }

        public IConsumePipeSpecification Specification => _specification;
        public IConsumePipeConfigurator Configurator => _specification;
    }
}
