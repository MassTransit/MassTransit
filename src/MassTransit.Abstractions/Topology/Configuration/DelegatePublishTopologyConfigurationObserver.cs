namespace MassTransit.Configuration
{
    public class DelegatePublishTopologyConfigurationObserver :
        IPublishTopologyConfigurationObserver
    {
        readonly IPublishTopologyConfigurator _publishTopology;

        public DelegatePublishTopologyConfigurationObserver(IPublishTopologyConfigurator publishTopology)
        {
            _publishTopology = publishTopology;
        }

        public void MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
            where T : class
        {
            IMessagePublishTopologyConfigurator<T> publishTopologyConfigurator = _publishTopology.GetMessageTopology<T>();

            configurator.AddDelegate(publishTopologyConfigurator);
        }
    }
}
