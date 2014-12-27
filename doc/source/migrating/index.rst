Migrating to Version 3
======================

Old Quick Start
"""""""""""""""

.. sourcecode:: csharp
    :linenos:

    public class YourMessage { public string Text { get; set; } }
    public class Program
    {
        public static void Main()
        {
            Bus.Initialize(sbc =>
            {
                sbc.UseMsmq();
                sbc.VerifyMsmqConfiguration();
                sbc.UseMulticastSubscriptionClient();
                sbc.ReceiveFrom("msmq://localhost/test_queue");
                sbc.Subscribe(subs=>
                {
                    subs.Handler<YourMessage>(msg=>Console.WriteLine(msg.Text));
                });
            });

            Bus.Instance.Publish(new YourMessage{Text = "Hi"});
        }
    }

New Quick Start
"""""""""""""""

.. sourcecode:: csharp
    :linenos:

    public class YourMessage { public string Text { get; set; } }
    public class Program
    {
        public static void Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMQ(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "test_queue", ep =>
                {
                    ep.Handler<YourMessage>(async context =>
                    {
                        Console.WriteLine("Received: {0}", context.Message.Text);
                    });
                });
            });

            var handle = await busControl.Start();

            busControl.Publish(new YourMessage{Text = "Hi"});

            handle.Stop();
        }
    }
