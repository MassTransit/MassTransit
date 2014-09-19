When messages are published, how do they get there?
===================================================

What actually happens when you call ``bus.Publish(yourMessage)``? The process
itself is pretty simple, but it does depend on your transport. The first thing
to know is that MassTransit has something called an 'Outbound Pipeline'. Messages
enter into the outbound pipeline to be sent to the actual transport. Once they
hit the transport, the messages leave your .Net process and enter the process
of the transport infrastructure.

.. note::

	Transports that are currently supported are MSMQ and RabbitMQ. There are
	also community contributed ActiveMQ and Azure Service Bus transports.
	Finally there is a non-production In-Memory Loopback which we use for 
	testing. 

MassTransit prefers a dynamic routing model, we do not enforce a static routing
model, however it can be achieved with the 'static routing bits'. What this 
means is that when you call the ``Subscribe`` methods, MassTransit is going to
setup the necessary routing for you. 

The 'necessary' routing will vary by transport that you use.

.. note::

	We do realize that some prefer a more static approach to this process, so there
	is an extension to the system to allow this called static routing. You will HAVE
	to manually set up all subscriptions on ALL endpoints for this to work. Its a lot
	of manual monkey work in my opinion, but a scalpel you shall have.

With that out of the way lets discuss how the transport affects routing.

Multicast MSMQ
--------------

Subscription are communicated to each bus through the use of multicast MSMQ. 
This allows us to not have single point of failure, but it does NOT tollerate
all of the bus instances going down. This is because the subscription information
is being held in memory.

Plain MSMQ
----------

Subscription data is communicated to each bus through an intermediary known
as the 'Subscription Service'. The subscription service is a well known location
where each bus sends its subscription requests to, and gets the subscription
requests of others from. 

Publisher
'''''''''

.. sourcecode:: csharp
    :linenos:

    // Setup Mass Transit.
    Bus.Initialize(sbc =>
    {
        sbc.UseMsmq();
        sbc.ReceiveFrom("msmq://localhost/BulkProcessing.Web");
        sbc.UseSubscriptionService("msmq://localhost/mt_subscriptions");
        // sbc.VerifyMsmqConfiguration(); This doesn't work on Windows 8.
    });
    
    // Send Message
    var sendContext = new SendContext<Message>(message);
    sendContext.SetMessageId(Guid.NewGuid().ToString());
    sendContext.SetCorrelationId(Guid.NewGuid().ToString());
    sendContext.SetExpirationTime(DateTime.Now.AddDays(1));
    
    Bus.Instance.Publish<Message>(message);

Subscriber
''''''''''

.. sourcecode:: csharp
    :linenos:

    static void Main(string[] args)
    {
        Bus.Initialize(sbc =>
        {
            sbc.UseMsmq();
            sbc.ReceiveFrom("msmq://localhost/BulkProcessing.Consumer");
            sbc.UseSubscriptionService("msmq://localhost/mt_subscriptions");
            // sbc.VerifyMsmqConfiguration(); This doesn't work on Windows 8.
    
            sbc.Subscribe(subs =>
            {
                subs.Handler<Message>(msg => Console.WriteLine(msg.Text));
            });
        });
    
        while (true)
        {
            Thread.Sleep(1000);
        }
    }

Internal detail of both MSMQ transports
----------------------------------------

Because MSMQ doesn't have any routing capabilities, MassTransit has built them
internal using a construct called a 'Pipeline.' This pipeline is configured by the
local subscription adapter (one for Plain MSMQ and one for Multicast MSMQ) to add
and remove segments to the pipeline. When a message comes in it goes through the
pipeline logic, and then is sent directly to the bus on the other end.

.. note::

	It is the subscription service that keeps all of the outbound and inbound pipelines, 
	across all of the instances,  in order.

RabbitMQ
--------

Because RabbitMQ has a much, much better routing system, instead of trying
to redo that work for RabbitMQ, we instead configure the RabbitMQ system's 
routing primitives to achieve the same thing that we have done in MSMQ
and the Outbound/Inbound pipelines.

So a message is routed straight to the correct RabbitMQ Exchange. The internal
workings of MassTransit make sure to configure RabbitMQ exchanges and bindings 
to implement the MassTransit pattern of routing. This means MT can make one call
to RMQ, and let RabbitMQ deal with it from there.
