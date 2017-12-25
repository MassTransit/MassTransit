namespace MassTransit.RabbitMqTransport.Topology.Configuration.Configurators
{
    using System;
    using System.Collections.Generic;


    public class QueueBindingConfigurator :
        QueueConfigurator,
        IQueueBindingConfigurator
    {
        public QueueBindingConfigurator(string name, string type, bool durable, bool autoDelete)
            : base(name, type, durable, autoDelete)
        {
            BindingArguments = new Dictionary<string, object>();
            RoutingKey = "";
        }

        public void SetBindingArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                BindingArguments.Remove(key);
            else
                BindingArguments[key] = value;
        }

        public string RoutingKey { get; set; }

        public IDictionary<string, object> BindingArguments { get; }
    }
}