namespace MassTransit
{
    /// <summary>
    /// Specifies a management endpoint, with the prefix "manage"
    /// </summary>
    public class ManagementEndpointDefinition :
        TemporaryEndpointDefinition
    {
        public ManagementEndpointDefinition()
            : base("manage")
        {
        }
    }
}
