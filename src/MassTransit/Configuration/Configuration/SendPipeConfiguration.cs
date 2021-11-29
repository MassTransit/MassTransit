namespace MassTransit.Configuration
{
    using Middleware;
    using Transports;


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
