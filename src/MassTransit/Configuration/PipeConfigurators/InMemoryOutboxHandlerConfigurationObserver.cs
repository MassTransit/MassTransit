namespace MassTransit.PipeConfigurators
{
    using ConsumeConfigurators;


    /// <summary>
    /// Configures a message retry for a handler, on the handler configurator, which is constrained to
    /// the message types for that handler, and only applies to the handler.
    /// </summary>
    public class InMemoryOutboxHandlerConfigurationObserver :
        IHandlerConfigurationObserver
    {
        void IHandlerConfigurationObserver.HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
        {
            var specification = new InMemoryOutboxSpecification<T>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
