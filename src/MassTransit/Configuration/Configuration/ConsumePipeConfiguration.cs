namespace MassTransit.Configuration
{
    using ConsumePipeSpecifications;


    public class ConsumePipeConfiguration :
        IConsumePipeConfiguration
    {
        readonly ConsumePipeSpecification _specification;

        public ConsumePipeConfiguration()
        {
            _specification = new ConsumePipeSpecification();
        }

        public ConsumePipeConfiguration(IConsumePipeSpecification parentSpecification)
            : this()
        {
            _specification.ConnectConsumePipeSpecificationObserver(new ParentConsumePipeSpecificationObserver(parentSpecification));
        }

        public IConsumePipeSpecification Specification => _specification;
        public IConsumePipeConfigurator Configurator => _specification;
    }
}
