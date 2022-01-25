namespace MassTransit.Configuration
{
    public class ConsumePipeConfiguration :
        IConsumePipeConfiguration
    {
        public ConsumePipeConfiguration(IConsumeTopology consumeTopology)
        {
            Specification = new ConsumePipeSpecification();
            Specification.ConnectConsumePipeSpecificationObserver(new TopologyConsumePipeSpecificationObserver(consumeTopology));
        }

        public ConsumePipeConfiguration(IConsumePipeSpecification parentSpecification)
        {
            Specification = parentSpecification.CreateConsumePipeSpecification();
            Specification.ConnectConsumePipeSpecificationObserver(new ParentConsumePipeSpecificationObserver(parentSpecification));
        }

        public IConsumePipeSpecification Specification { get; }

        public IConsumePipeConfigurator Configurator => Specification as IConsumePipeConfigurator;
    }
}
