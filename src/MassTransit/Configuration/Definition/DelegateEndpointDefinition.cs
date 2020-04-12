namespace MassTransit.Definition
{
    public class DelegateEndpointDefinition :
        DefaultEndpointDefinition
    {
        readonly string _endpointName;
        readonly IDefinition _definition;

        public DelegateEndpointDefinition(string endpointName, IDefinition definition)
        {
            _endpointName = endpointName;
            _definition = definition;
        }

        public override string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _endpointName;
        }

        public override int? ConcurrentMessageLimit => _definition.ConcurrentMessageLimit;
    }
}
