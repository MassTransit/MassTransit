Subscription Options
""""""""""""""""""""""""""""

MassTransit has a lot of ways that you can provide subscription options.


Subscription Options During Configuration
'''''''''''''''''''''''''''''''''''''''''

Now that we have a transport, an address, and some basic options figured out the meat of the work
is in front of you. Establishing your subscriptions. As you can see there are a lot of options
so I am going to save most of the explanation for the next page.

.. note::

    Permanent Subscriptions will NOT be automatically unsubscribed at bus shutdown. See :doc:`keyideas`

Handler
~~~~~~~

This is the simplest of the options. You simple register a lambda method that
will be called each time a message of type ``YourMessage`` arrives on the endpoint.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("queue_name", ep =>
        {
            ep.Handler<YourMessage>(async cxt => {});
            ep.Handler<YourMessage>(async cxt => {}, endpointConfig => {});
        });
    });


Instance
~~~~~~~~

Passing MassTransit an object instance, MassTransit will subscribe any public method
with a single input parameter and a void method.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("queue_name", ep =>
        {
            s.Instance(yourObject);
            s.Instance(yourObject, retryPolicy: Retry.None);

        });
    });


Consumer
~~~~~~~~

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("queue_name", ep =>
        {
            s.Consumer(()=> new YourConsumer() );
            s.Consumer(consumerFactory)
            s.Consumer(consumerType, type => Activator.CreateInstance(type));

        });
    });


Observer
~~~~~~~~

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("queue_name", ep =>
        {
            s.Observer(() => new YourObserver() );
            s.Observer(() => new YourObserver(), obsConfig => {});
        });
    });


Saga
~~~~

.. note::

    Currently only available with Automatonymous

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("queue_name", ep =>
        {
            s.Saga(sagaRepository)
        });
    });


LoadFrom
~~~~~~~~

.. note::

    Requires an IoC container.

If you are using an IoC container like AutoFac, StructureMap or Castle then MT
can scan your container for you to find consumers to be subscribed.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("abc", ep =>
        {
            //if using an IoC container
            //this will scan the container and call Consumer(type) on found
            //types
            ep.LoadFrom(container);
        });
    });

.. note::

    Need more notes here

Subscription Options During Post Configuration
''''''''''''''''''''''''''''''''''''''''''''''

.. note::

    Subscriptions established post-configuration are assumed to be transient. If this
    is to be a permanent subscription, it needs to be established during configuration.

.. sourcecode:: csharp

    var bus = Bus.Factory.CreateUsingInMemory(cfg => { /* configure */ });

    //options
    bus.ConnectHandler();
    bus.ConnectInstance();
    bus.ConnectConsumer();
    bus.ConnectSaga();

