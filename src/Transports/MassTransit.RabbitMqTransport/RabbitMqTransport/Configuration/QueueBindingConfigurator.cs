namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;


    public class QueueBindingConfigurator :
        RabbitMqQueueConfigurator,
        IRabbitMqQueueBindingConfigurator
    {
        protected QueueBindingConfigurator(string queueName, string exchangeType, bool durable, bool autoDelete)
            : base(queueName, exchangeType, durable, autoDelete)
        {
            BindingArguments = new Dictionary<string, object>();
            RoutingKey = "";
        }

        public IDictionary<string, object> BindingArguments { get; }

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

        public void AutoDeleteAfter(TimeSpan duration)
        {
            AutoDelete = true;
            QueueExpiration = duration;
        }
    }
}
