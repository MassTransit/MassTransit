namespace MassTransit
{
    /// <summary>
    /// Specifies a temporary endpoint, with the prefix "response"
    /// </summary>
    public class ResponseEndpointDefinition :
        TemporaryEndpointDefinition
    {
        public ResponseEndpointDefinition()
            : base("response")
        {
        }
    }
}
