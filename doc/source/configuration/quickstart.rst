Show me the code!
=================

All right, all right, already. Here you go. Below is a functional setup of
MassTransit.

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


So what is all of this doing?
"""""""""""""""""""""""""""""""""""

If we are going to create a messaging system, we need to create a message. ``YourMessage``
is a .Net class that will represent our message. Notice that it's just a plain
C# class (or POCO).

Next up, we need a program to run our code. Here we have a standard issue
command line ``Main`` method. To setup the bus we start with the static
class ``Bus`` and its ``Initialize`` method. This method takes a lambda whose
first and only argument is a class that will let you configure every aspect
of the bus.

One of your first decisions is going to be "What transport do I want to run on?"
Here we have choosen MSMQ (``sbc.UseMsmq()``) because its easy to install on a
Windows machines (``sbc.VerifyMsmqConfiguration()``), will do just that
and its most likely what you will use.

After that we have the ``sbc.UseMulticastSubscriptionClient()`` this tells the
bus to pass subscription information around using PGM over MSMQ giving us a
way to talk to all of the other bus instances on the network. This eliminates
the need for a central control point.

Now we have the ``sbc.ReceiveFrom("msmq://localhost/test_queue)`` line which
tells us to listen for new messages at the local, private, msmq queue 'test_queue'.
So anytime a message is placed into that queue the framework will process the
message and deliver it to any consumers subscribed on the bus.

Lastly, in the configuration, we have the Subscribe lambda, where we have
configured a single ``Handler`` for our message which will be activated which
each message of type ``YourMessage`` and will print to the console.

And now we have a bus that is configured and can do things. So now we can grab
the singleton instance of the service bus and call the ``Publish`` method on it.


But Singletons are Evil!
""""""""""""""""""""""""""""""""""""

If you shudder at the thought of a singleton in your code, that's ok - we have
you covered too. Instead of using ``Bus.Initialize`` you can use the code below:

.. sourcecode:: csharp
    :linenos:

    var bus = ServiceBusFactory.New(sbc =>
    {
        sbc.UseMsmq();
        sbc.UseMulticastSubscriptionClient();
        sbc.ReceiveFrom("msmq://localhost/test_queue");
    });
