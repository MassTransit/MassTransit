namespace MassTransit.RabbitMqTransport.Topology.Configurators
{
    using System;
    using System.Collections.Generic;
    using Entities;
    using RabbitMQ.Client;


    public class QueueConfigurator :
        ExchangeConfigurator,
        IQueueConfigurator,
        Queue
    {
        public QueueConfigurator(string queueName, string exchangeType, bool durable, bool autoDelete)
            : base(queueName, exchangeType, durable, autoDelete)
        {
            QueueArguments = new Dictionary<string, object>();

            QueueName = queueName;
        }

        public QueueConfigurator(QueueConfigurator source, string name)
            : base(name, source.ExchangeType, source.Durable, source.AutoDelete)
        {
            QueueArguments = new Dictionary<string, object>();

            QueueName = name;
        }

        public void SetQueueArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                QueueArguments.Remove(key);
            else
                QueueArguments[key] = value;
        }

        public void SetQueueArgument(string key, TimeSpan value)
        {
            var milliseconds = (int)value.TotalMilliseconds;

            SetQueueArgument(key, milliseconds);
        }

        public bool Lazy
        {
            set => SetQueueArgument("x-queue-mode", value ? "lazy" : "default");
        }

        public void EnablePriority(byte maxPriority)
        {
            QueueArguments[Headers.XMaxPriority] = (int)maxPriority;
        }

        public bool Exclusive { get; set; }
        public TimeSpan? QueueExpiration { get; set; }

        public string QueueName { get; set; }

        public IDictionary<string, object> QueueArguments { get; }

        public override RabbitMqEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return new RabbitMqEndpointAddress(hostAddress, ExchangeName ?? QueueName, ExchangeType, Durable, AutoDelete);
        }
    }
}
