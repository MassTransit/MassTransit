using System;
using MassTransit.ServiceBus;
using MassTransit.ServiceBus.Util;

namespace Starbucks.Messages
{
    [Serializable]
    public class NewOrderMessage : CorrelatedBy<Guid>
    {
        public NewOrderMessage(string name, string drink, string size)
        {
            CorrelationId = CombGuid.NewCombGuid();
            Name = name;
            Item = drink;
            Size = size;
        }

        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Item { get; set;}
        public string Size { get; set;}
    }    
}
