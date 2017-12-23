namespace MassTransit.RabbitMqTransport.Topology.Configurators
{
    using System;
    using System.Collections.Generic;


    public class ExchangeConfigurator :
        IExchangeConfigurator
    {
        public ExchangeConfigurator(string exchangeName, string exchangeType, bool durable, bool autoDelete)
        {
            ExchangeName = exchangeName;
            ExchangeType = exchangeType;
            Durable = durable;
            AutoDelete = autoDelete;

            ExchangeArguments = new Dictionary<string, object>();
        }

        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }

        public IDictionary<string, object> ExchangeArguments { get; }

        public void SetExchangeArgument(string key, object value)
        {
            if (value != null)
                ExchangeArguments[key] = value;
            else
                ExchangeArguments.Remove(key);
        }

        public void SetExchangeArgument(string key, TimeSpan value)
        {
            int milliseconds = (int)value.TotalMilliseconds;

            SetExchangeArgument(key, milliseconds);
        }

        protected virtual IEnumerable<string> GetQueryStringOptions()
        {
            if (!Durable)
                yield return "durable=false";
            if (AutoDelete)
                yield return "autodelete=true";
            if (ExchangeType != RabbitMQ.Client.ExchangeType.Fanout)
                yield return "type=" + ExchangeType;
        }

    }
}