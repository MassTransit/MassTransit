How are subscriptions shared?
"""""""""""""""""""""""""""""

Once a subscription is created on a local bus, this information then needs to be shared between all the different bus instances in your application network. 

Say you have a Timeout service. This service runs on a separate bus that is hooked to the queue msmq://localhost/timeout. Whenever your application wants to schedule a timeout, it needs to send 
a ``ScheduleTimeout`` message to this queue. But in order for your application bus to know this routing path, it needs to receive this information first. 

Though the routing data is the same, how this information get to all of the nodes is different depending on your transport configuration.


Permanent v.s. Temporary Subscriptions
''''''''''''''''''''''''''''''''''''''

Subscriptions are what registers the intent to consume a given message. The structure of the 
subscription is ``Subscription(message_name, address, correlation_id)``. Permanent subscriptions
represent a subscription that you want to have stay around even if your process is shut down
(maybe you are doing an upgrade and don't want to miss a message). A temporary subscription
is to be used in the case where you won't care if you miss a message while shut down. 


Unsubscribe Token
'''''''''''''''''

Any time you call subscribe off of ``IServiceBus`` you will get a token back that can be called
to unsubscribe the subscription.

.. sourcecode:: csharp

    var subscriptionToken = bus.Subscribe<MyMessage>();
    
    //later, when you want to unsubscribe
    if(!subscriptionToken())
    {
       //handle failure condition 
    }

.. note::

    If you are using a permanent subscription and don't want unsubscribe don't call the delegate.
    
    If you do call the token it will be re-subscribed on start up.


Transport Specific Notes
''''''''''''''''''''''''
As said earlier, different transport mechanisms use different subscription distributions. Please take a look below how your transport layer handles this and to see the differences. 
If your transport layer does not handle this specifically, you can rely on the ``SubscriptionService``

MSMQ Multicast
--------------

.. warning::

    - limited by default to one subnet
    - subscriptions do not survive service restarts (no perm subscriptions)
    - sensitive to the order in which services are brought online

Each bus instance communicates with every other instance on the network through a reliable
multicast network protocol.

.. sourcecode:: csharp

    var bus = ServiceBusFactory.New(sbc =>
    {
        //other settings
        
        sbc.UseMsmq();       
        sbc.UseMulticastSubscriptionClient(); //turns on multicast
        sbc.SetNetwork("YOUR_KEY"); //must be set to cross machines
    });
    
MSMQ Runtime Services
---------------------

Each bus instance communicates with every other instance through an intermediary known as
the Runtime Services (specifically the Subscription Service). 

.. note::

    supports permanent subscriptions

.. sourcecode:: csharp

    var bus = ServiceBusFactory.New(sbc =>
    {
        //other settings
        
        sbc.UseMsmq();       
        sbc.UseSubscriptionService("msmq://localhost/my_queue");
    });	
	
RabbitMQ
--------

Each bus instance communicates with a local rabbitmq server. Setting up the necessary
bindings and queues based on MassTransit conventions. RabbitMQ then syncs all binding
information to all nodes in the cluster.

.. note::

    supports permanent subscriptions
    
.. sourcecode:: csharp

    var bus = ServiceBusFactory.New(sbc =>
    {
        // this is the recommended routing strategy, and will call 'sbc.UseRabbitMq()'.
        sbc.UseRabbitMqRouting();
        
        // more config
    });
	
	
Subscription Service
''''''''''''''''''''
If your transport layer does not provide a transport specific way of sharing subscription information you can use the ``SubscriptionService``. In this case subscription coordination depends on a central manager. 
This manager is an instance of a ``SubscriptionService`` that runs somewhere in your network. Each bus instance then uses a ``SubscriptionClient`` to communicate with this central management and exchanges subscription information.

By default MassTransit bundles with the ``MSMQ Runtime Services``. This is an MSMQ implementation of the ``SubscriptionService`` that you can run seperatly from your project. 
If you do not use the MSMQ as a transport layer you can easily host it yourself.


Hosting a subscription service
------------------------------

The example below creates two 'application domain' bus instances and a subscription service. The service  bus is responsible for transporting timeout messages, the second is your own awesome application.

.. sourcecode:: csharp

	//
	// setup the subscription service
	//
	var subscriptionBus = ServiceBusFactory.New(sbc =>
	{
		sbc.UseStomp();       
		sbc.SetConcurrentConsumerLimit(1);
		
		sbc.ReceiveFrom("stomp://localhost/mt_subscriptions");
	});
	
    var subscriptionSagas = new InMemorySagaRepository<SubscriptionSaga>();
    var subscriptionClientSagas = new InMemorySagaRepository<SubscriptionClientSaga>();
    var subscriptionService = new SubscriptionService(subscriptionBus, subscriptionSagas, subscriptionClientSagas);

	subscriptionService.Start();
	
	//
	// setup the time out service
	//
	var timeoutBus = ServiceBusFactory.New(sbc =>
	{
		sbc.UseStomp();       
		sbc.UseControlBus();

		sbc.ReceiveFrom("stomp://localhost/mt_timeouts");
		sbc.UseSubscriptionService("stomp://localhost/mt_subscriptions");
	});

	var timeoutService = new TimeoutService(timeoutBus, new InMemorySagaRepository<TimeoutSaga>());
    timeoutService.Start();

	//
	// setup your awesome application bus
	//
    var bus = ServiceBusFactory.New(sbc =>
    {
		sbc.UseStomp();       
        sbc.UseControlBus();		
		
        sbc.ReceiveFrom("stomp://localhost/your_awesome_application");
        sbc.UseSubscriptionService("stomp://localhost/mt_subscriptions");    
	});

	
Subscription Client
-------------------

By stating ``sbc.UseSubscriptionService("stomp://localhost/mt_subscriptions");`` you implicitly attach a ``SubscriptionClient`` to your service bus. 

One of the first thing this client does, is send a ``AddSubscriptionClient`` to the ``SubscriptionService`` queue. After that it starts observing subscription 
modifications and subsequently sends either an ``AddSubscription`` or ``RemoveSubscription`` messsage. This way updates are propagated to other nodes in your application network.

The client also handles the ``SubscriptionRefresh`` messages it receives from the ``SubscriptionService``. These refresh messages contain the subscription information of other nodes.


