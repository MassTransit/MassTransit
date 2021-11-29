namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using RabbitMQ.Client;
    using Topology;


    public class RabbitMqQueueConfigurator :
        RabbitMqExchangeConfigurator,
        IRabbitMqQueueConfigurator,
        Queue
    {
        protected RabbitMqQueueConfigurator(string queueName, string exchangeType, bool durable, bool autoDelete)
            : base(queueName, exchangeType, durable, autoDelete)
        {
            QueueArguments = new Dictionary<string, object>();

            QueueName = queueName;
        }

        public void SetQuorumQueue(int? replicationFactor)
        {
            SetQueueArgument(Headers.XQueueType, "quorum");
            Durable = true;
            Exclusive = false;

            QueueArguments.Remove(Headers.XMaxPriority);

            if (replicationFactor.HasValue)
            {
                if (replicationFactor.Value < 1)
                    throw new ArgumentOutOfRangeException(nameof(replicationFactor), "Must be greater than zero and less than or equal to the cluster size.");

                SetQueueArgument(Headers.XQuorumInitialGroupSize, replicationFactor.Value);
            }
        }

        public bool SingleActiveConsumer
        {
            set
            {
                if (value)
                    SetQueueArgument(Headers.XSingleActiveConsumer, true);
                else
                    QueueArguments.Remove(Headers.XSingleActiveConsumer);
            }
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
            set => SetQueueArgument(Headers.XQueueMode, value ? "lazy" : "default");
        }

        public void EnablePriority(byte maxPriority)
        {
            QueueArguments[Headers.XMaxPriority] = (int)maxPriority;
        }

        public bool Exclusive { get; set; }

        public TimeSpan? QueueExpiration
        {
            get
            {
                if (QueueArguments.TryGetValue("x-expires", out var value) && value is long milliseconds)
                    return TimeSpan.FromMilliseconds(milliseconds);

                return null;
            }
            set
            {
                if (value.HasValue && value.Value > TimeSpan.Zero)
                    QueueArguments["x-expires"] = (long)value.Value.TotalMilliseconds;
                else
                    QueueArguments.Remove("x-expires");
            }
        }

        public string QueueName { get; set; }
        public IDictionary<string, object> QueueArguments { get; }

        public override RabbitMqEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return new RabbitMqEndpointAddress(hostAddress, ExchangeName ?? QueueName, ExchangeType, Durable, AutoDelete);
        }
    }
}
