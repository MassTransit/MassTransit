namespace MassTransit.Configuration
{
    using Util;


    public class HandlerConfigurationObservable :
        Connectable<IHandlerConfigurationObserver>,
        IHandlerConfigurationObserver
    {
        public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class
        {
            ForEach(observer => observer.HandlerConfigured(configurator));
        }
    }
}
