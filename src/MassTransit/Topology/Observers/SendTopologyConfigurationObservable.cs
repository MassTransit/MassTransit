namespace MassTransit.Topology.Observers
{
    using GreenPipes.Util;


    public class SendTopologyConfigurationObservable :
        Connectable<ISendTopologyConfigurationObserver>,
        ISendTopologyConfigurationObserver
    {
        public void MessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> configuration)
            where T : class
        {
            All(observer =>
            {
                observer.MessageTopologyCreated(configuration);

                return true;
            });
        }
    }
}
