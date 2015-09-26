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
	
