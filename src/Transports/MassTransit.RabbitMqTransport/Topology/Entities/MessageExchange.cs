namespace MassTransit.RabbitMqTransport.Topology.Entities
{
    using System.Collections.Generic;


    public class MessageExchange :
        Exchange
    {
        public MessageExchange(string name, string type, bool durable, bool autoDelete)
        {
            ExchangeName = name;
            ExchangeType = type;
            Durable = durable;
            AutoDelete = autoDelete;

            ExchangeArguments = new Dictionary<string, object>();
        }

        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> ExchangeArguments { get; }
    }
}
