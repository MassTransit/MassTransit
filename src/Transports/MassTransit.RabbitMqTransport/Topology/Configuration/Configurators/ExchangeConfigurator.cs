namespace MassTransit.RabbitMqTransport.Topology.Configurators
{
    using System;
    using System.Collections.Generic;
    using Entities;


    public class ExchangeConfigurator :
        IExchangeConfigurator,
        Exchange
    {
        public ExchangeConfigurator(string exchangeName, string exchangeType, bool durable = true, bool autoDelete = false)
        {
            ExchangeName = exchangeName;
            ExchangeType = exchangeType;
            Durable = durable;
            AutoDelete = autoDelete;

            ExchangeArguments = new Dictionary<string, object>();
        }

        public ExchangeConfigurator(Exchange source)
        {
            ExchangeName = source.ExchangeName;
            ExchangeType = source.ExchangeType;
            Durable = source.Durable;
            AutoDelete = source.AutoDelete;

            ExchangeArguments = new Dictionary<string, object>(source.ExchangeArguments);
        }

        public string ExchangeName { get; set; }

        public IDictionary<string, object> ExchangeArguments { get; }
        public string ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }

        public void SetExchangeArgument(string key, object value)
        {
            if (value != null)
                ExchangeArguments[key] = value;
            else
                ExchangeArguments.Remove(key);
        }

        public void SetExchangeArgument(string key, TimeSpan value)
        {
            var milliseconds = (int)value.TotalMilliseconds;

            SetExchangeArgument(key, milliseconds);
        }

        public virtual RabbitMqEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return new RabbitMqEndpointAddress(hostAddress, ExchangeName, ExchangeType, Durable, AutoDelete);
        }
    }
}
