namespace MassTransit.Configuration
{
    using Util;


    public class MessageTopologyConfigurationObservable :
        Connectable<IMessageTopologyConfigurationObserver>,
        IMessageTopologyConfigurationObserver
    {
        public void MessageTopologyCreated<T>(IMessageTopologyConfigurator<T> configuration)
            where T : class
        {
            ForEach(observer => observer.MessageTopologyCreated(configuration));
        }
    }
}
