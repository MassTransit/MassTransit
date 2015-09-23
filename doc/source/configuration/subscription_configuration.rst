Subscription Configuration
""""""""""""""""""""""""""

MassTransit supports a variety of message consumers, each of which is detailed below. There are 
some common configuration settings which are available across all consumer types, which are documented
separately.

Once the transport, host, and bus-level configuration settings have been specified, the meat of the work
is in front of you -- adding your subscriptions. As you can see below there are a lot of options
so I am going to save most of the explanation for the next page.


Receive Endpoint Subscriptions
''''''''''''''''''''''''''''''

A recieve endpoint specifies a queue on which messages should be received. When a consumer is
subscribed to a receive endpoint, a subscription is created for every message type handled by
the consumer. A subscription is persistent and remains in place after the bus is stopped.
See :doc:`keyideas`

Handler
~~~~~~~

A handler is the simpliest consumer type, and allows a method, delegate method, or lambda method to be
specified that is called for every message of the specified type that is received on the endpoint.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("input_queue", ep =>
        {
            ep.Handler<MessageA>(async context => 
            {
                // do something with message
            });

            ep.Handler<MessageB>(context => 
            {
                // do something
                return Task.Completed;
            }, handlerCfg => 
            {
                // specify a retry interval of 100ms with 5 attempts
                handlerCfg.UseRetry(Retry.Interval(5, 100));
            });
        });
    });

    busControl.Start();


Consumer
~~~~~~~~

A consumer is registered using the ``Consumer`` syntax, of which there are several overloads.
All of the overload ultimately boil down to some type of ``IConsumerFactory``, which is used
to construct and send the message to the consumer.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("input_queue", ep =>
        {
            // default constructor
            s.Consumer<YourConsumer>();

            // delegate factory method
            s.Consumer(() => new YourConsumer());

            // pass a custom consumer factory
            s.Consumer(consumerFactory)

            // an type-based factory that returns an object (container friendly)
            s.Consumer(consumerType, type => Activator.CreateInstance(type));

            // delegate factory method
            s.Consumer(() => new YourConsumer(), x =>
            {
                // inject middleware, etc.
                x.UseLog(ConsoleOut, async context => "Consumer created");
            });
        });
    });


Instance
~~~~~~~~

An instance accepts an existing object instance of a consumer, otherwise it is identical.
There are fewer overloads, because, seriously, it takes ``object``. It's not rocket science.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("input_queue", ep =>
        {
            s.Instance(yourObject);

            s.Instance(yourObject, x => { Middlewarez! });
        });
    });


Observer
~~~~~~~~

An object, which implements at least one ``IObserver<T>``, can be registered in the same way
that a consumer can be registered.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("input_queue", ep =>
        {
            s.Observer(() => new YourObserver());
            s.Observer(() => new YourObserver(), obsConfig => {});
        });
    });


Saga
~~~~

There are two types of sagas, and each of them are registered a different way. The first, a
legacy-style saga, is registered using the ``Saga`` method. The message types in the saga
interfaces are subscribed to the endpoint.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("input_queue", ep =>
        {
            // the saga repository is an already created ISagaRepository<T>
            // where T is the saga class type
            s.Saga(sagaRepository)
        });
    });

The more powerful version of a saga, powered by Automatonymous, is a state machine saga. A 
state machine saga consists of a state type and a state machine, combined with a saga repository.
The event types in the state machine (denoted by the ``Event<T>`` property type) are subscribed
to the endpoint.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("input_queue", ep =>
        {
            // machine is an instance of the state machine class
            // the saga repository is an already created ISagaRepository<T>
            // where T is the saga class type
            s.StateMachineSaga(machine, sagaRepository)
        });
    });


LoadFrom
~~~~~~~~

.. note::

    Requires an IoC container.

If you are using an IoC container like AutoFac, StructureMap, or Castle Windsor, MassTransit
can scan your container to find consumers and subscribe those consumers to the endpoint. Some 
containers even have methods to help build the container, by scanning the assembly for classes
that implement any IConsumer interface, and register the concrete consumer type.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
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


Bus Connections
'''''''''''''''

Once the bus has been created, the receive endpoints are created at that point and cannot be modified.
However, the bus itself has a temporary queue which can be used for transient message consumer
connections. In most cases this is usuable for occasional events (such as a cache update or some other
low frequency event), and also can be used to receive responses (from the request/response client)
or faults (failures in message processing).

.. note::

    Consumers connected to the bus post-configured are transient. Persistent subscriptions are not
    supported on the bus queue (because it's temporary, it's gone soon after the process is gone).

.. sourcecode:: csharp

    var bus = Bus.Factory.CreateUsingInMemory(cfg => { /* configure */ });

    bus.ConnectHandler<MessageA>();
    bus.ConnectInstance(yourConsumer);
    bus.ConnectConsumer<YourConsumer>();
    bus.ConnectSaga(sagaRepository);
    bus.ConnectStateMachineSaga(machine, sagaRepository);

.. note::

    It should also be noted that published events cannot be received by the bus queue, as no 
    bindings are created on bus connections. Messages to the bus queue must be sent directly 
    to the queue (such as a response, or a request fault). So many of the above connection
    methods are there for completeless only (IE, connecting a state machine saga to a bus queue
    is downright silly, but hey, completeness wins).

