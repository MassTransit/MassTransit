namespace MassTransit
{
    /// <summary>
    /// Specifies a temporary endpoint, with the prefix "response"
    /// </summary>
    public class ControlEndpointDefinition :
        TemporaryEndpointDefinition
    {
        public ControlEndpointDefinition()
            : base("control")
        {
        }
    }
}
