namespace MassTransit.Configuration
{
    using Pipeline;
    using Pipeline.Pipes;
    using SendPipeSpecifications;
    using Topology;
    using Topology.Observers;


    public class SendPipeConfiguration :
        ISendPipeConfiguration
    {
        readonly SendPipeSpecification _specification;

        public SendPipeConfiguration(ISendTopology sendTopology)
        {
            _specification = new SendPipeSpecification();
            _specification.ConnectSendPipeSpecificationObserver(new TopologySendPipeSpecificationObserver(sendTopology));
        }

        public SendPipeConfiguration(ISendPipeSpecification parentSpecification)
        {
            _specification = new SendPipeSpecification();
            _specification.ConnectSendPipeSpecificationObserver(new ParentSendPipeSpecificationObserver(parentSpecification));
        }

        public ISendPipeSpecification Specification => _specification;
        public ISendPipeConfigurator Configurator => _specification;

        public ISendPipe CreatePipe()
        {
            return new SendPipe(_specification);
        }
    }
}
