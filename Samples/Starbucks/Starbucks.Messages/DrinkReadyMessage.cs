using System;
using MassTransit.ServiceBus;

namespace Starbucks.Messages
{
    [Serializable]
    public class DrinkReadyMessage : CorrelatedBy<string>
    {
        public DrinkReadyMessage(string name, string drink)
        {
            CorrelationId = name;
            Drink = drink;
        }

        public string Drink { get; set; }

        //this property is just to be explicit about what our external correlation id is
        public string Name
        {
            get { return CorrelationId; }
        }

        public string CorrelationId { get; private set; }
    }
}