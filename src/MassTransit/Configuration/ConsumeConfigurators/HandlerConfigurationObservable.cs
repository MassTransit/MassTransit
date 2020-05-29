namespace MassTransit.ConsumeConfigurators
{
    using GreenPipes.Util;


    public class HandlerConfigurationObservable :
        Connectable<IHandlerConfigurationObserver>,
        IHandlerConfigurationObserver
    {
        public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class
        {
            All(observer =>
            {
                observer.HandlerConfigured(configurator);

                return true;
            });
        }
    }
}
