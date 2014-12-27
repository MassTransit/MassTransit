Common Configuration Options
""""""""""""""""""""""""""""

Trying to get multiple applications to talk to each other is not a simple
problem. The biggest difficulty seems to be just getting everything configured
correctly. With over five years of experience in setting up message based systems
the developers on MassTransit have tried their darndest to make sure that the
MassTransit's defaults cover the majority of the decisions you will have to make
while minimizing the configuration code you have to deal with. We hope that the options
are clear and make sense why you need to select them. Below are some of the options you
have:


Transport Factory Options
'''''''''''''''''''''''''

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg => {});
    Bus.Factory.CreateUsingRabbitMQ(cfg => {});
    //coming soon
    //Bus.Factory.CreateUsingAzureServiceBus(cfg => {});

The first decision is what transport are you going to use? RabbitMQ or Azure Service Bus? If you don't know
which one to choose I suggest reading up on the two and see which one works better for
your environment.

Basic Options
'''''''''''''

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("test_queue", ep =>
        {

        });
    });

The next decision we have to make is what address are we going to listen on? Here
we are using the In-Memory transport and specifing an queue
to receive messages on.

.. warning::

    In-Memory is for testing only.

The same endpoint but for rabbit mq.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/a_virtual_host"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint(host, "test_queue", ep =>
        {

        });
    });

Serializer Options
''''''''''''''''''

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        //receive code options

        cfg.UseBinarySerializer();
        cfg.UseBsonSerializer();
        cfg.UseJsonSerializer();
        cfg.UseXmlSerializer();
    });

This is mostly optional, because the transports will set their preferred defaults, but if you
need to override the default you can using these methods. With the ``SetDefaultSerializer`` you can
provide a custom serializer that you created.

Logging Options
''''''''''''''''''

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        //receive code options

        cfg.UseLog(Console.Out, async cxt =>
        {
            return string.Format("Consumed Message Id: {0}", cxt.MessageId);
        });
    });

Bus Tuning Options
''''''''''''''''''

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.SetConcurrentConsumerLimit(2);
        cfg.SetDefaultTransactionTimeout(5.Minutes());

        cfg.AfterConsumingMessage(()=>{});
        cfg.BeforeConsumingMessage(()=>{}):

        cfg.ConfigureEndpoint();
    });

These options, aren't usually needed until you get into production and need to tune the
behavior of the bus.

Turning on Diagnostics
''''''''''''''''''''''

If you want to get a snapshot of how your service bus is configured, you can get
a pretty good picture of it by using the method.

.. sourcecode:: csharp

	var bus = Bus.Factory.CreateUsingInMemory(cfg => { /* usual stuff */ });
	var probe = bus.Probe();
	//you can now inspect the probe

	//for your convience we also have added a few helper methods.
	bus.WriteIntrospectionToFile("a_file.txt"); //great to send with support requests :)
	bus.WriteIntrospectionToConsole();

You may also want to inspect a running bus instance remotely. For that you just need to enable
remote introspection like so.

.. sourcecode:: csharp

	Bus.Factory.CreateUsingInMemory(cfg =>
	{
	    //the usual options

		cfg.EnableRemoteInstrospection();
	});

You can then use the ``busdriver`` to query the status. using:

	busdriver status -uri:<address to control bus>

Low Level Config Api
''''''''''''''''''''

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.AddBusConfigurator
        cfg.AddService<TService>();
    });

If you are using these, then we probably need to talk. This usually means that there is a low
level feature we are not supplying to you. Its totally ok to use these, but they tend to
need a lot of parameters and require intimate knowledge of MassTransit.
