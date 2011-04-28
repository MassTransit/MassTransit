namespace MassTransit.Tests.Configuration
{
    using System;
    using TestFramework.Examples.Messages;

    public class SubscriptionSpike
    {
        public void DesignSurface()
        {
            Bus.Initialize(null, cfg=>
            {
                int i = 0;
                cfg.Subscribe(sc=>
                {
                    sc.Instance(new PingConsumer()).Permanent();
                    sc.Instance(new PingConsumer());

                    
                    
                    sc.Handler<Ping>(ping=>Console.WriteLine(ping));

                    sc.Consumer(typeof(object), t=>new object());

                    sc.Consumer<PingConsumer>(type => new PingConsumer());
                });
            });
        }

        class PingConsumer : Consumes<Ping>.All
        {
            public void Consume(Ping message)
            {
                throw new NotImplementedException();
            }
        }
    }
}