namespace MassTransit.RabbitMqTransport.Topology.Configuration.Configurators
{
    using System;
    using System.Collections.Generic;


    public class QueueConfigurator :
        ExchangeConfigurator,
        IQueueConfigurator
    {
        public QueueConfigurator(string name, string type, bool durable, bool autoDelete)
            : base(name, type, durable, autoDelete)
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
            int milliseconds = (int)value.TotalMilliseconds;

            SetQueueArgument(key, milliseconds);
        }

        public bool Lazy
        {
            set { SetQueueArgument("x-queue-mode", value ? "lazy" : "default"); }
        }

        public void EnablePriority(byte maxPriority)
        {
            QueueArguments["x-max-priority"] = (int)maxPriority;
        }

        public string QueueName { get; set; }
        public bool Exclusive { get; set; }
        public IDictionary<string, object> QueueArguments { get; }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            foreach (var option in base.GetQueryStringOptions())
            {
                yield return option;
            }

            if (Exclusive)
                yield return "exclusive=true";
        }
    }
}