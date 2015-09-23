Migrating from MassTransit v2.x to MassTransit 3
======================

To migrate an application built with an earlier version of MassTransit, there are a few changes that need to be 
considered. 

.. note::

    MSMQ is not supported in MassTransit 3. If you are still using MSMQ, you will need to remain on
    the 2.x version or migrate to RabbitMQ or Azure Service Bus.



The MassTransit v2.x API
---------------

In MassTransit 2.x, a bus was created for a transport using the ``ServiceBusFactory`` syntax.

.. sourcecode:: csharp

    public class Program
    {
        public static void Main()
        {
            IServiceBus bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseRabbitMq();
                sbc.ReceiveFrom("rabbitmq://localhost/input_queue");
                sbc.Subscribe(subs =>
                {
                    subs.Consumer<MyConsumer>();
                });
            });

            PublishMessage(bus);

            bus.Dispose();
        }

        public static void PublishMessage(IServiceBus bus)
        {
            bus.Publish(new YourMessage { Text = "Hi" });
        }
    }


The MassTransit 3 API
-----------------

The syntax for creating a bus using MassTransit 3 is different, where the transport is an initial
decision point that must be made. The default retry policy should also be specified, as the default
is no longer five attempts before moving to the error queue.

.. sourcecode:: csharp

    public class Program
    {
        public static void Main()
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.UseRetry(Retry.Immediate(5));

                sbc.ReceiveEndpoint(host, "input_queue", ep =>
                {
                    ep.Consumer<MyConsumer>();
                });
            });

            busControl.Start();

            PublishMessage(bus)
                .Wait();

            busControl.Stop();
        }

        public static Task PublishMessage(IBus bus)
        {
            return bus.Publish(new YourMessage { Text = "Hi" });
        }
    }


Major Changes
--------------

There are several API changes to consider, so they are summarized here.

IServiceBus to IBus
~~~~~~~~~~~~~~~~~~~

The ``IServiceBus`` interface is gone, replaced with ``IBus``. This breaking change was done to ensure that in the
switch to the new async methods that there were no accidental "didn't await" situations.

Also, ``IBus`` is really just a collection of other interfaces. In this case, it's unlikely that any part of the an
application would ever need to take a dependency on ``IBus`` directly, but should instead opt for a narrower interface,
such as ``ISendEndpointProvider`` or ``IPublishEndpoint``. Each has a particular usefulness, but should only be used
in cases where there is not an existing context which can be used.


Consumes<T>.* to IConsumer<T>
~~~~~~~~~~~~~~~~~~~~~~~~~

The clever ``Consumes<T>.All`` (and the related ``Consumes<T>.Context``) are no longer supported. Instead, consumers
should now use the single ``IConsumer<T>`` interface.

.. sourcecode:: csharp

    class AbConsumer :
        IConsumer<A>,
        IConsumer<B>
    {
        public async Task Consume(ConsumeContext<A> context)
        {
        }
    
        public async Task Consume(ConsumeContext<B> context)
        {
        }
    }

All consumer methods are now async and include the ``ConsumeContext<T>`` argument. The ``context`` parameter is
incredibly useful, and should be used for anything message related. Both ``IPublishEndpoint`` and ``ISendEndpointProvider``
are implemented by the context, and should be used to send or publish messages. Doing so ensures that the ``ConversationId``
and ``InitiatorId`` are properly carried through the system.


Receive Endpoints
~~~~~~~~~~~~~~~~

In MassTransit v2, a separate bus had to be created for every queue. With MassTransit 3, that is no longer the case. Any number
of receive endpoints can be configured on a single bus, reducing the overhead and memory usage, as well as the number of 
broker connections. This really helps with broker performance, as well as simplifies configuration.

It's also completely legal to create a bus with no receive endpoints. In this case, the bus is meant only for publish/send, as 
well as request/response. A temporary queue is created for the bus, on which responses can be received.


State Machine Sagas
~~~~~~~~~~~~~~~~~~~

Automatonymous is the only support state machine saga format with MassTransit 3. Magnum has been completely eradicated from
the code base, with the new state machine engine being the go-forward standard. The integration with Automatonymous is great,
including a specialized ``MassTransitStateMachine`` class, to allow advanced messages features such as request/response and
timeouts to be supported.

Courier
~~~~~~~

The routing slip engine is now built into the main assembly, and has been updated to support event subscriptions (instead of 
just publishing all routing slip events). The routing slips are not backwards compatible, as the syntax has been improved
to support better troubleshooting and event history. The API is mostly the same, though, so it's easy to migrate.


Living Document
---------------

While the above items are just a few of the changes, this document will continue to be updated in response to questions about
how to migrate code using previous features arise. 

































