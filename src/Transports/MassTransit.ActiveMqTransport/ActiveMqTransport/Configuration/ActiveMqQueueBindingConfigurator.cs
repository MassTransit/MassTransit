namespace MassTransit.ActiveMqTransport.Configuration
{
    public class ActiveMqQueueBindingConfigurator :
        ActiveMqQueueConfigurator,
        IActiveMqQueueBindingConfigurator
    {
        protected ActiveMqQueueBindingConfigurator(string queueName, bool durable, bool autoDelete)
            : base(queueName, durable, autoDelete)
        {
        }

        public string Selector { get; set; }
    }
}
