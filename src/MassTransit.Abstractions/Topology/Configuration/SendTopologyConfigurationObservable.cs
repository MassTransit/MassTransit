namespace MassTransit.Configuration
{
    using Util;


    public class SendTopologyConfigurationObservable :
        Connectable<ISendTopologyConfigurationObserver>,
        ISendTopologyConfigurationObserver
    {
        public void MessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> configuration)
            where T : class
        {
            ForEach(observer => observer.MessageTopologyCreated(configuration));
        }
    }
}
