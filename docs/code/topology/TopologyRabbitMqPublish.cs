namespace TopologyRabbitMqPublish
{
    using System;
    using TopologyContracts;
    using MassTransit;

    public class Program
    {
        public static void Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Publish<OrderSubmitted>(x =>
                {
                    x.Durable = false; // default: true
                    x.AutoDelete = true; // default: false
                    x.ExchangeType = "fanout"; // default, allows any valid exchange type
                });

                cfg.Publish<OrderEvent>(x =>
                {
                    x.Exclude = true; // do not create an exchange for this type
                });
            });
        }
    }
}
