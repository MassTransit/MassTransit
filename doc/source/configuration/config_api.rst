Common Configuration Options
""""""""""""""""""""""""""""

Trying to get multiple applications to talk to each other is not a simple
problem. The biggest difficulty seems to be just getting everything configured
correctly. With over three years of experience in setting up message based systems
the developers on MassTransit have tried their darndest to make sure that the
MassTransit's defaults cover the majority of the decisions you will have to make
while minimizing the configuration code you have to deal with. We hope that the options
are clear and make sense why you need to select them. Below are some of the options you
have:


Transport Factory Options
'''''''''''''''''''''''''

.. sourcecode:: csharp

    ServiceBusFactory.New(sbc =>
    {
        sbc.UseMsmq();
        sbc.UseRabbitMq();
        
        //if you would like to implement your own.
        sbc.AddTransportFactory<TTransportFactory>();
    });

The first decision is what transport are you going to use? RabbitMQ or MSMQ? If you don't know
which one to choose I suggest reading up on the two and see which one works better for
your environment.

Basic Options
'''''''''''''

.. sourcecode:: csharp

    ServiceBusFactory.New(sbc =>
    {
        //transport choice
        
        sbc.ReceiveFrom("address");
        sbc.UseControlBus();
    });

The next decision we have to make is what address are we going to listen on? Addresses are in the
form of a URL and will look like ``msmq://localhost/queue_name`` or for RabbitMQ 
``rabbitmq://localhost/queue_name``.

.. warning:: 

    Each instance must have its own address. For more information see 'gotchas'

The last concern is, do you want to use a control bus or not. ``Control Bus``

.. sourcecode:: csharp

    ServiceBusFactory.New(sbc =>
    {
        //transport choice
        //address

        sbc.UseControlBus();
    });

Serializer Options
''''''''''''''''''

.. sourcecode:: csharp

    ServiceBusFactory.New(sbc =>
    {
        //transport choice
        //address
        //control bus
        
        sbc.UseBinarySerializer();
        sbc.UseBsonSerializer();
        sbc.UseJsonSerializer();
        sbc.UseVersionOneXmlSerializer();
        sbc.UseXmlSerializer();
        
        //if you would like to implement your own.
        sbc.SetDefaultSerializer<TSerializer>();
    });

This is mostly optional, because the transports will set their preferred defaults, but if you
need to override the default you can using these methods. With the ``SetDefaultSerializer`` you can
provide a custom serializer that you created.

    
Bus Tuning Options
''''''''''''''''''

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc =>
  {
    sbc.SetConcurrentConsumerLimit(2);
    sbc.SetDefaultTransactionTimeout(5.Minutes());

    sbc.AfterConsumingMessage(()=>{});
    sbc.BeforeConsumingMessage(()=>{}):

    sbc.ConfigureEndpoint();
  });

These options, aren't usually needed until you get into production and need to tune the 
behavior of the bus.

Turning on Diagnostics
''''''''''''''''''''''

If you want to get a snapshot of how your service bus is configured, you can get 
a pretty good picture of it by using the method.

.. sourcecode:: csharp

	var bus = ServiceBusFactory.New(sbc => { /* usual stuff */ });
	var probe = bus.Probe();
	//you can now inspect the probe
	
	//for your convience we also have added a few helper methods.
	bus.WriteIntrospectionToFile("a_file.txt"); //great to send with support requests :)
	bus.WriteIntrospectionToConsole();

You may also want to inspect a running bus instance remotely. For that you just need to enable
remote introspection like so.

.. sourcecode:: csharp

	ServiceBusFactory.New(sbc =>
	{
	    //the usual options
		
		sbc.EnableRemoteInstrospection();
	});

You can then use the ``busdriver`` to query the status. using:

	busdriver status -uri:<address to control bus>

Low Level Config Api
''''''''''''''''''''

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc =>
  {
    sbc.AddBusConfigurator
    sbc.AddService<TService>();
  });

If you are using these, then we probably need to talk. This usually means that there is a low
level feature we are not supplying to you. Its totally ok to use these, but they tend to 
need a lot of parameters and require intimate knowledge of MassTransit.
