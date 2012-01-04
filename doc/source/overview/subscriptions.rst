How are subscriptions shared?
"""""""""""""""""""""""""""""

Subscriptions are created and then shared to every other bus instance. Though the data 
is the same, how they get to all of the nodes is different depending on your configuration.


Permanent vs Temporary Subscriptions
''''''''''''''''''''''''''''''''''''

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
    
    If you do call the token it will be resubscribed on start up.

Transport Specific Notes
''''''''''''''''''''''''

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

Each bus instance communicates with every other intance through an intermediary known as
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