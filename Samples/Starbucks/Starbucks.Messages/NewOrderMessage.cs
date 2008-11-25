using System;

namespace Starbucks.Messages
{
    using MassTransit;
    using MassTransit.Util;

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
