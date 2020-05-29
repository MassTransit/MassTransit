namespace MassTransit.Topology.Observers
{
    using GreenPipes.Util;


    public class MessageTopologyConfigurationObservable :
        Connectable<IMessageTopologyConfigurationObserver>,
        IMessageTopologyConfigurationObserver
    {
        public void MessageTopologyCreated<T>(IMessageTopologyConfigurator<T> configuration)
            where T : class
        {
            All(observer =>
            {
                observer.MessageTopologyCreated(configuration);

                return true;
            });
        }

        public void MessagePropertyTopologyCreated<TMessage, T>(IMessagePropertyTopologyConfigurator<TMessage, T> configuration)
            where TMessage : class
        {
            All(observer =>
            {
                observer.MessagePropertyTopologyCreated(configuration);

                return true;
            });
        }
    }
}
