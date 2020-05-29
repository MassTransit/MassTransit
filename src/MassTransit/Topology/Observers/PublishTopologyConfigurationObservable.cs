namespace MassTransit.Topology.Observers
{
    using GreenPipes.Util;


    public class PublishTopologyConfigurationObservable :
        Connectable<IPublishTopologyConfigurationObserver>,
        IPublishTopologyConfigurationObserver
    {
        public void MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
            where T : class
        {
            All(observer =>
            {
                observer.MessageTopologyCreated(configurator);

                return true;
            });
        }
    }
}
