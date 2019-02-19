namespace MassTransit.PipeConfigurators
{
    using ConsumeConfigurators;
    using Pipeline.Filters.ConcurrencyLimit;


    /// <summary>
    /// Configures a concurrency limit for a handler, on the handler configurator, which is constrained to
    /// the message type for that handler, and only applies to the handler.
    /// </summary>
    /// <typeparam name="TMessage">The handler message type</typeparam>
    public class ConcurrencyLimitHandlerConfigurationObserver<TMessage> :
        IHandlerConfigurationObserver
        where TMessage : class
    {
        readonly IHandlerConfigurator<TMessage> _configurator;
        readonly IConcurrencyLimiter _limiter;

        public ConcurrencyLimitHandlerConfigurationObserver(IHandlerConfigurator<TMessage> configurator, int concurrentMessageLimit, string id = null)
        {
            _configurator = configurator;
            _limiter = new ConcurrencyLimiter(concurrentMessageLimit, id);
        }

        public IConcurrencyLimiter Limiter => _limiter;

        public void HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
            where T : class
        {
            var specification = new ConcurrencyLimitConsumePipeSpecification<TMessage>(_limiter);

            _configurator.AddPipeSpecification(specification);
        }
    }
}
