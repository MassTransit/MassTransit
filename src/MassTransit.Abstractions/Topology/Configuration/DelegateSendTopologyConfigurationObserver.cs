namespace MassTransit.Configuration
{
    public class DelegateSendTopologyConfigurationObserver :
        ISendTopologyConfigurationObserver
    {
        readonly ISendTopology _sendTopology;

        public DelegateSendTopologyConfigurationObserver(ISendTopology sendTopology)
        {
            _sendTopology = sendTopology;
        }

        public void MessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> configuration)
            where T : class
        {
            IMessageSendTopologyConfigurator<T> specification = _sendTopology.GetMessageTopology<T>();

            configuration.AddDelegate(specification);
        }
    }
}
