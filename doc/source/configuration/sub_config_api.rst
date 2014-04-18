Common Subscription Options
""""""""""""""""""""""""""""

MassTransit has a lot of ways that you can provide subscription options. 


Subscription Options During Configuration
'''''''''''''''''''''''''''''''''''''''''

.. sourcecode:: csharp

    ServiceBusFactory.New(sbc =>
    {
        sbc.Subscribe(s=>
        {
            s.Handler(msg => {});
            s.Handler((cxt, msg) => {});
            
            s.Instance(yourObject);
            
            s.Consumer(()=> new YourConsumer() );
            s.Consumer(consumerFactory);
            s.Consumer(consumerType);
            s.Consumer<TConsumer>();
            
            //for a permanent subscription
            s.Consumer<TConsumer>()
                .Permanent();
            
            s.Saga(sagaRepository);
            
            //if using an IoC container 
            //this will scan the container and call Consumer(type) on found
            //types
            s.LoadFrom(container);
        });
    });

Now that we have a transport, an address, and some basic options figured out the meat of the work
is in front of you. Establishing your subscriptions. As you can see there are a lot of options
so I am going to save most of the explanation for the next page.

.. note:: 

    Permanent Subscriptions will NOT be automatically unsubscribed at bus shutdown. See :doc:`keyideas`


Subscription Options With IoC Container
''''''''''''''''''''''''''''''''''''''''''''''

.. sourcecode:: csharp

    ServiceBusFactory.New(sbc =>
    {
        sbc.Subscribe(s=>
        {
            //if using an IoC container 
            //this will scan the container and call Consumer(type) on found
            //types
            s.LoadFrom(container);
        });
    });

.. note::

    Need more notes here
    
Subscription Options During Post Configuration
''''''''''''''''''''''''''''''''''''''''''''''

.. sourcecode:: csharp

    var bus = ServiceBusFactory.New(sbc => { /* configure */ });
    
    //options
    bus.SubscribeConsumer();
    bus.SubscribeHandler();
    bus.SubscribeInstance();
    bus.SubscribeSaga();

.. note::

    Subscriptions established post-configuration are assumed to be transient. If this
    is to be a permanent subscription, it needs to be established during configuration.
