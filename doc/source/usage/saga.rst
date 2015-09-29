Creating state machine sagas with Automatonymous
================================================

Sagas are a powerful way to coordinate a long-lived, distributed transaction. And MassTransit has top notch support for defining
sagas using event-driven state machines. This is made possible through tight integration with Automatonymous_. Automatonymous is a 
state machine engine written by Chris Patterson, aka, PhatBoyG.

.. _Automatonymous: https://github.com/MassTransit/Automatonymous

What is a saga?
---------------

A saga is a long-lived transaction managed by a coordinator. Sagas are stateful entities which orchestrate events, changing state as
events are observed, and maintaining the overall state of the transaction. Sagas were designed to manage the complexity of a distributed
transaction without locking or immediate consistency. They manage state and are able to identify when a transaction completed successfully
or if it faulted, necessitating compensation.

We didn’t create it, we learned it from the `original Cornell paper`_ and from Arnon Rotem-Gal-Oz's `description`_.

.. _original Cornell paper: http://www.cs.cornell.edu/andru/cs711/2002fa/reading/sagas.pdf
.. _description: http://www.rgoarchitects.com/Files/SOAPatterns/Saga.pdf


Defining a state machine saga
-----------------------------

To define a state machine saga, create a class that inherits from ``MassTransitStateMachine``. The ``MassTransit.Automatonymous`` NuGet package
must be referenced.

.. note::
    
    This section is written using the shopping cart sample, which is `hosted on GitHub`_.

.. _hosted on GitHub: https://github.com/MassTransit/Sample-ShoppingWeb

.. sourcecode:: csharp

    public class ShoppingCartStateMachine :
        MassTransitStateMachine<ShoppingCart>
    {
        public ShoppingCartStateMachine()
        {
            InstanceState(x => x.CurrentState);

The state machine class, and the specification of the property for the current state of the machine are defined first.

.. sourcecode:: csharp

        Event(() => ItemAdded, x => x.CorrelateBy(cart => cart.UserName, context => context.Message.UserName)
            .SelectId(context => Guid.NewGuid()));

The event that is observed when an item is added to the cart, along with the correlation between the state machine instance and the message are defined. The id generator for the saga instance is also defined.

.. sourcecode:: csharp

        Event(() => Submitted, x => x.CorrelateById(context => context.Message.CartId));

The order submitted event, and the correlation for that order.

.. sourcecode:: csharp

        Schedule(() => CartExpired, x => x.ExpirationId, x =>
        {
            x.Delay = TimeSpan.FromSeconds(10);
            x.Received = e => e.CorrelateById(context => context.Message.CartId);
        });

In order to schedule the timeout, a schedule is defined, including the time delay for the scheduled event, and the correlation of the event back to the state machine.

Now, it is time for the actual behavior of the events and how they interact with the state of the _ShoppingCart_.

.. sourcecode:: csharp

        Initially(
            When(ItemAdded)
                .Then(context =>
                {
                    context.Instance.Created = context.Data.Timestamp;
                    context.Instance.Updated = context.Data.Timestamp;
                    context.Instance.UserName = context.Data.UserName;
                })
                .ThenAsync(context => Console.Out.WriteLineAsync($"Item Added: {context.Data.UserName} to {context.Instance.CorrelationId}"))
                .Schedule(CartExpired, context => new CartExpiredEvent(context.Instance))
                .TransitionTo(Active)
            );

Initially defined events that can create a state machine instance. In the above, the properties of the instance are initialized, and then the _CartExpired_ event is scheduled, after which the state is set to _Active_.

.. sourcecode:: csharp

        During(Active,
            When(Submitted)
                .Then(context =>
                {
                    if (context.Data.Timestamp > context.Instance.Updated)
                        context.Instance.Updated = context.Data.Timestamp;
                    context.Instance.OrderId = context.Data.OrderId;
                })
                .ThenAsync(context => Console.Out.WriteLineAsync($"Cart Submitted: {context.Data.UserName} to {context.Instance.CorrelationId}"))
                .Unschedule(CartExpired)
                .TransitionTo(Ordered),

While the shopping cart is active, if the order is submitted, the expiration is canceled (via _Unschedule_) and the state is set to Ordered.

.. sourcecode:: csharp

            When(ItemAdded)
                .Then(context =>
                {
                    if (context.Data.Timestamp > context.Instance.Updated)
                        context.Instance.Updated = context.Data.Timestamp;
                })
                .ThenAsync(context => Console.Out.WriteLineAsync($"Item Added: {context.Data.UserName} to {context.Instance.CorrelationId}"))
                .Schedule(CartExpired, context => new CartExpiredEvent(context.Instance)),

If another item is added to the cart, the _CartExpired_ event is scheduled, and the existence of a previously scheduled event (via the _ExpirationId_ property) is used to cancel the previously scheduled event.

.. sourcecode:: csharp

            When(CartExpired.Received)
                .ThenAsync(context => Console.Out.WriteLineAsync($"Item Expired: {context.Instance.CorrelationId}"))
                .Publish(context => new CartRemovedEvent(context.Instance))
                .Finalize()
            );

If the _CartExpired_ event is received, the cart removed event is published and the shopping cart is finalized.

.. sourcecode:: csharp

            SetCompletedWhenFinalized();
        }

Signals that the state machine instance should be deleted if it is finalized. This is used to tell Entity Framework to delete the row from the database.

.. sourcecode:: csharp

        public State Active { get; private set; }
        public State Ordered { get; private set; }

The states of the shopping cart (_Initial_ and _Final_ are built-in states).

.. sourcecode:: csharp

        public Schedule<ShoppingCart, CartExpired> CartExpired { get; private set; }

The schedule definition for the CartExpired event.

.. sourcecode:: csharp

        public Event<CartItemAdded> ItemAdded { get; private set; }
        public Event<OrderSubmitted> Submitted { get; private set; }
    }

The events that are observed by the state machine (the correlations are defined earlier in the state machine).

The state machine is generic, and requires a state class (because sagas are stateful), so that is defined below. The state class has the values
that are persisted between events.

.. sourcecode:: csharp

    class ShoppingCartState :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

The CorrelationId is the primary key of the saga state instance. It is assigned either from a property on the initial message that creates
the saga instance, or can be generated using ``NewId.NextGuid()``, which ensures a nice ordered sequential identifier.

.. sourcecode:: csharp

        public string CurrentState { get; set; }

The current state of the saga, which can be saved as a *string* or an *int*, depending upon your database requirements. An *int* is smaller,
but requires that all valid states be mapped to integers during the definition of the state machine.

.. sourcecode:: csharp

        public Guid? ExpirationId { get; set; }

This is an identifier that is used by the state machine's scheduling feature, to capture the scheduled message identifier. Message scheduling within
sagas is a powerful feature, which is described later.

.. sourcecode:: csharp

        public string UserName { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }


        public Guid? OrderId { get; set; }
    }

The remainder of the properites are relevant to the application, and are saved when properly mapped using the saga repository (which can be any supported
storage engine, Entity Framework and NHibernate are supported out of the box).


Connecting the saga to a receive endpoint
-----------------------------------------

To connect the state machine saga to a receive endpoint, a saga repository is used, along with the state machine instance.

.. sourcecode:: csharp

    var repository = new InMemorySagaRepository<ShoppingCartState>();

    _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
    {
        IRabbitMqHost host = x.Host(...);

        x.ReceiveEndpoint(host, "shopping_cart_state", e =>
        {
            e.PrefetchCount = 8;
            e.StateMachineSaga(_machine, repository);
        });

        x.UseInMemoryMessageScheduler(); // for testing, to make it easy
    });


Combining events (think Fork/Join)
----------------------------------

Multiple events can be combined into a single event, for the purposes of joining together multiple operations. To define a combined event, the ``Event`` 
syntax has an overload.

// TBD


