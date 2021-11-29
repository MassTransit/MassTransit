namespace MassTransit.Configuration
{
    public class NamedEndpointDefinition :
        DefaultEndpointDefinition
    {
        readonly string _endpointName;

        public NamedEndpointDefinition(string endpointName)
        {
            _endpointName = endpointName;
        }

        public override string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _endpointName;
        }
    }
}
