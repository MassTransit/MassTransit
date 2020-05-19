namespace MassTransit.ActiveMqTransport.Topology.Configurators
{
    public class QueueBindingConfigurator :
        QueueConfigurator,
        IQueueBindingConfigurator
    {
        protected QueueBindingConfigurator(string queueName, bool durable, bool autoDelete)
            : base(queueName, durable, autoDelete)
        {
        }

        public string Selector { get; set; }
    }
}
