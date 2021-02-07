namespace MassTransit.Registration
{
    public interface IEndpointRegistration
    {
        IEndpointDefinition GetDefinition(IConfigurationServiceProvider provider);
    }
}
