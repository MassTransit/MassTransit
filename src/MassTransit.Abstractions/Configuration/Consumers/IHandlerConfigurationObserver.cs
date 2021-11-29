namespace MassTransit
{
    using System.ComponentModel;


    public interface IHandlerConfigurationObserver
    {
        /// <summary>
        /// Called when a consumer/message combination is configured
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class;
    }
}
