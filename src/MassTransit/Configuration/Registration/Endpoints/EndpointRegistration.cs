namespace MassTransit.Registration
{
    public class EndpointRegistration<T> :
        IEndpointRegistration
        where T : class
    {
        public IEndpointDefinition GetDefinition(IConfigurationServiceProvider provider)
        {
            return provider.GetRequiredService<IEndpointDefinition<T>>();
        }
    }
}
