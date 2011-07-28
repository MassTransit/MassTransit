Common Configuration Options
""""""""""""""""""""""""""""

Trying to get multiple applications all talking to each other is not a simple
problem. There are a lot of edge cases that have to be taken into account. With over 
three years of experience is getting message based systems to talk to each other 
the developers have tried their darndest to make sure MassTransit's defaults cover
the majority of the decisions minimizing the configuration code you have to deal with.
Even so there are some minimum bits that need to be configured.

Transport Factory Options
'''''''''''''''''''''''''

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc =>
  {
    sbc.AddTransportFactory<TTransportFactory>();

    //sbc.UseLocalLoopback(); an implicit option
    sbc.UseMsmq();
    sbc.UseRabbitMq();
  });

The first option is what transport are you going to use? RabbitMQ or MSMQ? If you don't know see the 
section on the transport choices to help you in your quest to know.

Basic Options
'''''''''''''

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc =>
  {
    sbc.ReceiveFrom("address");

    sbc.UseControlBus();
  });

The next decisions are going to be what address this bus instance is going to listen on.

NOTE: Each instance must have its own address unless you plan on a competing consumer model:
see ``Competing Consumers``. After that you need to choose

The last basic concern is, do you want to use a control bus or not. ``Control Bus``


Subscription Options
''''''''''''''''''''

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc =>
  {
    sbc.Subscribe();
  });

Now that we have a transport, an address, and some basic options figured out you need to set 
up your subscriptions.

Serializer Options
''''''''''''''''''

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc =>
  {
	  sbc.SetDefaultSerializer(); //how to set your very own
    sbc.UseBinarySerializer();
    sbc.UseBsonSerializer();
    sbc.UseJsonSerializer();
    sbc.UseVersionOneXmlSerializer();
    sbc.UseXmlSerializer();
  });

This is mostly optional, and the transports will set their preferred defaults, but if you have
a choice you can set it yourself as well. With the ``SetDefaultSerializer`` you can even
use a custom one that you created.

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

Low Lever Config Api
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
