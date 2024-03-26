namespace MassTransit.Configuration
{
    using Util;


    public class PublishTopologyConfigurationObservable :
        Connectable<IPublishTopologyConfigurationObserver>,
        IPublishTopologyConfigurationObserver
    {
        public void MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
            where T : class
        {
            ForEach(observer => observer.MessageTopologyCreated(configurator));
        }

        public void Method4()
        {
        }

        public void Method5()
        {
        }

        public void Method6()
        {
        }
    }
}
