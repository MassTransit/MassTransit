namespace MassTransit.Definition
{
    public class DelegateEndpointDefinition :
        DefaultEndpointDefinition
    {
        readonly IDefinition _definition;
        readonly string _endpointName;

        public DelegateEndpointDefinition(string endpointName, IDefinition definition)
        {
            _endpointName = endpointName;
            _definition = definition;
        }

        public override int? ConcurrentMessageLimit => _definition.ConcurrentMessageLimit;

        public override string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _endpointName;
        }
    }
}
