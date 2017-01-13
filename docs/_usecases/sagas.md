---
layout: default
title: track long running transactions? (sagas)
subtitle: Composable middleware for the Task Parallel Library
---

How to build long running transactions using a saga.

## Using Sagas

The ability to ochestrate a series of events is a powerful feature, and MassTransit makes this possible.

A saga is a long-lived transaction managed by a coordinator. Sagas are initiated by an event, sagas orchestrate events, and sagas maintain the state of the overall transaction. Sagas are designed to manage the complexity of a distributed transaction without locking and immediate consistency. They manage state and track any compensations required if a partial failure occurs.

We didn’t create it, we learned it from the [original Cornell paper](http://www.cs.cornell.edu/andru/cs711/2002fa/reading/sagas.pdf) and from Arnon Rotem-Gal-Oz's [description](http://www.rgoarchitects.com/Files/SOAPatterns/Saga.pdf).

* ToC
{:toc}

## Creating Automatonymous State Machines


[Automatonymous](https://github.com/MassTransit/Automatonymous) is a state machine library built by the same team that created MassTransit. Automatonymous provides a friendly syntax for declaring a state machine, including the states, events (both trigger event and data events are supported), and behaviors. The simple syntax makes it easy to get started with your own state machines, while including many advanced features that make it extremely flexible in a variety of business contexts.

Like MassTransit, Automatonymous is free, open source, and licensed under the very permissive Apache 2.0 license, making usable at no cost to anyone for both commercial and non-commercial use.

### Automatonymous Quick Start


So you've got the chops and want to get started quickly using Automatonymous. Maybe
you are a bad ass and can't be bothered with reading documentation, or perhaps you
are already familiar with the Magnum StateMachine and want to see what things have
changed. Either way, here it is, your first state machine configured using Automatonymous.

{% highlight C# %}
    class Relationship
    {
        public State CurrentState { get; set; }
        public string Name { get; set; }
    }

    class RelationshipStateMachine :
        MassTransitStateMachine<Relationship>
    {
        public RelationshipStateMachine()
        {
            Event(() => Hello);
            Event(() => PissOff);
            Event(() => Introduce);

            State(() => Friend);
            State(() => Enemy);

            Initially(
                When(Hello)
                    .TransitionTo(Friend),
                When(PissOff)
                    .TransitionTo(Enemy),
                When(Introduce)
                    .Then(ctx => ctx.Instance.Name = ctx.Data.Name)
                    .TransitionTo(Friend)                   
            );
        }

        public State Friend { get; private set; }
        public State Enemy { get; private set; }

        public Event Hello { get; private set; }
        public Event PissOff { get; private set; }
        public Event<Person> Introduce { get; private set; }
    }

    class Person
    {
        public string Name { get; set; }
    }
{% endhighlight %}

### Seriously?

Okay, so two classes are defined above, one that represents the state (`Relationship`)
and the other that defines the behavior of the state machine (`RelationshipStateMachine`).
For each state machine that is defined, it is expected that there will be at least one instance.
In Automatonymous, state is separate from behavior, allowing many instances to be managed using
a single state machine.

{% note %}
For some object-oriented purists, this may be causing the hair to raise on the back of your neck. Chill out, it's not the end of the world here. If you have a penchant for encapsulating behavior with data (practices such as domain model, DDD, etc.), recognize that programming language constructs are the only thing in your way here.
{% endnote %}

### Tracking State

State is managed in Automatonymous using a class, shown above as the `Relationship`.

### Defining Behavior

Behavior is defined using a class that inherits from `MassTransitStateMachine`. The class is generic,
and the state type associated with the behavior must be specified. This allows the state machine configuration
to use the state for a better configuration experience.

{% note %}
It also makes Intellisense work better.
{% endnote %}

States are defined in the state machine as properties. They are initialized by default, so there is no need
to declare them explicitly unless they are somehow special, such as a Substate or Superstate.

{% note %}
Configuration of a state machine is done using an internal DSL, using an approach known as Object Scoping, and is explained in Martin Fowler's Domain Specific Languages book.
{% endnote %}

### Creating Instances

tbd

### Creating the State Machine

tbd

### Raising Events


Once a state machine and an instance have been created, it is necessary to raise an event on the state
machine instance to invoke some behavior. There are three or four participants involved in raising an event: a
state machine, a state machine instance, and an event. If the event includes data, the data for the event is also
included.

The most explicit way to raise an event is shown below.

{% highlight C# %}
    var relationship = new Relationship();
    var machine = new RelationshipStateMachine();

    await machine.RaiseEvent(relationship, machine.Hello);
{% endhighlight %}

If the event has data, it is passed along with the event as shown.

{% highlight C# %}

    var person = new Person { Name = "Joe" };

    await machine.RaiseEvent(relationship, machine.Introduce, person);
{% endhighlight %}

### Lifters

Lifters allow events to be raised without knowing explicit details about the state machine or the instance type,
making it easier to raise events from objects that do not have prior type knowledge about the state machine or the instance. Using an approach known as *currying* (from functional programming), individual arguments of raising an event can be removed.

For example, using an event lift, the state machine is removed.

{% highlight C# %}
    var eventLift = machine.CreateEventLift(machine.Hello);

    // elsewhere in the code, the lift can be used    
    await eventLift.Raise(relationship);
{% endhighlight %}

The instance can also be lifted, making it possible to raise an event without any instance type knowledge.

{% highlight C# %}
    var instanceLift = machine.CreateInstanceLift(relationship);
    var helloEvent = machine.Hello;

    // elsewhere in the code, the lift can be used
    await instanceLift.Raise(helloEvent);
{% endhighlight %}

Lifts are commonly used by plumbing code to avoid dynamic methods or delegates, making code
clean and fast.

## Sample State Machine

The following code will require installing a few NuGet dependencies.

* `MassTransit`
* `Automatonymous`
* `MassTransit.Automatonymous`

## Defining a state machine saga

To define a state machine saga, create a class that inherits from `MassTransitStateMachine`. The `MassTransit.Automatonymous` NuGet package
must be referenced.

{% note %}
This section is written using the shopping cart sample, which is [hosted on GitHub](https://github.com/MassTransit/Sample-ShoppingWeb).
{% endnote %}

{% highlight C# %}
    public class ShoppingCartStateMachine :
        MassTransitStateMachine<ShoppingCart>
    {
        public ShoppingCartStateMachine()
        {
            InstanceState(x => x.CurrentState);
{% endhighlight %}

The state machine class, and the specification of the property for the current state of the machine are defined first.

{% highlight C# %}
        Event(() => ItemAdded, x => x.CorrelateBy(cart => cart.UserName, context => context.Message.UserName)
            .SelectId(context => Guid.NewGuid()));
{% endhighlight %}

The event that is observed when an item is added to the cart, along with the correlation between the state machine instance and the message are defined. The id generator for the saga instance is also defined.

{% highlight C# %}
        Event(() => Submitted, x => x.CorrelateById(context => context.Message.CartId));
{% endhighlight %}

The order submitted event, and the correlation for that order.

{% highlight C# %}
        Schedule(() => CartExpired, x => x.ExpirationId, x =>
        {
            x.Delay = TimeSpan.FromSeconds(10);
            x.Received = e => e.CorrelateById(context => context.Message.CartId);
        });
{% endhighlight %}

In order to schedule the timeout, a schedule is defined, including the time delay for the scheduled event, and the correlation of the event back to the state machine.

Now, it is time for the actual behavior of the events and how they interact with the state of the *ShoppingCart*.

{% highlight C# %}
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
{% endhighlight %}

Initially defined events that can create a state machine instance. In the above, the properties of the instance are initialized, and then the *CartExpired* event is scheduled, after which the state is set to *Active*.

{% highlight C# %}
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
{% endhighlight %}

While the shopping cart is active, if the order is submitted, the expiration is canceled (via *Unschedule*) and the state is set to Ordered.

{% highlight C# %}
        When(ItemAdded)
            .Then(context =>
            {
                if (context.Data.Timestamp > context.Instance.Updated)
                    context.Instance.Updated = context.Data.Timestamp;
            })
            .ThenAsync(context => Console.Out.WriteLineAsync($"Item Added: {context.Data.UserName} to {context.Instance.CorrelationId}"))
            .Schedule(CartExpired, context => new CartExpiredEvent(context.Instance)),
{% endhighlight %}

If another item is added to the cart, the *CartExpired* event is scheduled, and the existence of a previously scheduled event (via the *ExpirationId* property) is used to cancel the previously scheduled event.

{% highlight C# %}
        When(CartExpired.Received)
            .ThenAsync(context => Console.Out.WriteLineAsync($"Item Expired: {context.Instance.CorrelationId}"))
            .Publish(context => new CartRemovedEvent(context.Instance))
            .Finalize()
        );
{% endhighlight %}

If the *CartExpired* event is received, the cart removed event is published and the shopping cart is finalized.

{% highlight C# %}
            SetCompletedWhenFinalized();
        }
{% endhighlight %}

Signals that the state machine instance should be deleted if it is finalized. This is used to tell Entity Framework to delete the row from the database.

{% highlight C# %}
    public State Active { get; private set; }
    public State Ordered { get; private set; }
{% endhighlight %}

The states of the shopping cart (*Initial* and *Final* are built-in states).

{% highlight C# %}
    public Schedule<ShoppingCart, CartExpired> CartExpired { get; private set; }
{% endhighlight %}

The schedule definition for the CartExpired event.

{% highlight C# %}
    public Event<CartItemAdded> ItemAdded { get; private set; }
    public Event<OrderSubmitted> Submitted { get; private set; }
}
{% endhighlight %}

The events that are observed by the state machine (the correlations are defined earlier in the state machine).

The state machine is generic, and requires a state class (because sagas are stateful), so that is defined below. The state class has the values
that are persisted between events.

{% highlight C# %}
class ShoppingCartState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
{% endhighlight %}

The CorrelationId is the primary key of the saga state instance. It is assigned either from a property on the initial message that creates
the saga instance, or can be generated using `NewId.NextGuid()`, which ensures a nice ordered sequential identifier.

{% highlight C# %}
    public string CurrentState { get; set; }
{% endhighlight %}

The current state of the saga, which can be saved as a *string* or an *int*, depending upon your database requirements. An *int* is smaller,
but requires that all valid states be mapped to integers during the definition of the state machine.

{% highlight C# %}
    public Guid? ExpirationId { get; set; }
{% endhighlight %}

This is an identifier that is used by the state machine's scheduling feature, to capture the scheduled message identifier. Message scheduling within
sagas is a powerful feature, which is described later.

{% highlight C# %}
    public string UserName { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }


    public Guid? OrderId { get; set; }
}
{% endhighlight %}

The remainder of the properites are relevant to the application, and are saved when properly mapped using the saga repository (which can be any supported
storage engine, Entity Framework and NHibernate are supported out of the box).

## Connecting the saga to a receive endpoint

To connect the state machine saga to a receive endpoint, a saga repository is used, along with the state machine instance.

{% highlight C# %}
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
{% endhighlight %}

### Combining events (think Fork/Join)

Multiple events can be combined into a single event, for the purposes of joining together multiple operations. To define a combined event, the `Event` syntax has an overload.

{% highlight C# %}
public Event<OrderReady> Ready { get; private set; }
public Event<PaymentApproved> Approved { get; private set; }
public Event<StockVerified> Verified { get; private set; }

CompositeEvent(() => OrderReady, x => x.OrderReadyStatus, PaymentApproved, StockVerified);
{% endhighlight %}

Once both events have been delivered to the state machine, the third event, *OrderReady*, will be triggered.

{% note %}
The order of events being declared can impact the order in which they execute. Therefore, it is best to declare composite events at the end of the state machine declaration, after all other events and behaviors are declared. That way, the composite events will be raised *after* the dependent event behaviors.
{% endnote %}

## Persisting Saga Instances


Sagas are stateful event-based message consumers -- they retain state. Therefore, saving state between events is important. Without persistent state, a saga would consider each event a new event, and orchestration of subsequent events would be meaningless.

### Identity

Saga instances are identified by a unique identifier (`Guid`), represented by the `CorrelationId` on the saga instance. Events are correlated to the saga instance using either the unique identifier, or alternatively using an expression that correlates properties on the saga instance to each event. If the `CorrelationId` is used, it's always a one-to-one match, either the saga already exists, or it's a new saga instance. With a correlation expression, the expression might match to more than one saga instance, so care should be used -- because the event would be delivered to all matching instances.

{% danger %}
Seriously, don't sent an event to all instances -- unless you want to watch your messages consumers lock your entire saga storage engine.
{% enddanger %}

### Storage Engines

MassTransit supports several storage engines, including NHibernate, Entity Framework, and MongoDB. Each of these are setup in a similar way, but examples are shown below for each engine.

#### Entity Framework

Entity Framework seems to be the most common ORM for class-SQL mappings, and SQL is still widely used for storing data. So it's a win to have it supported out of the box by MassTransit. The code-first mapping example below shows the basics of getting started.

{% highlight C# %}
public class SagaInstance :
    SagaStateMachineInstance
{
    public SagaInstance(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    protected SagaInstance()
    {
    }

    public string CurrentState { get; set; }
    public string ServiceName { get; set; }
    public Guid CorrelationId { get; set; }
}


public class SagaInstanceMap :
    SagaClassMapping<SagaInstance>
{
    public SagaInstanceMap()
    {
        Property(x => x.CurrentState);
        Property(x => x.ServiceName, x => x.Length(40));
    }
}
{% endhighlight %}

The repository is then created on the context factory for the `DbContext` is available.

{% highlight C# %}
SagaDbContextFactory contextFactory = () =>
    new SagaDbContext<SagaInstance, SagaInstanceMap>(_connectionString);

var repository = new EntityFrameworkSagaRepository<SagaInstance>(contextFactory);
{% endhighlight %}

#### MongoDB

MongoDB is an easy to use saga repository, because setup is easy. There is no need for class mapping, the saga instances can be persisted easily using a MongoDB collection.

{% highlight C# %}
public class SagaInstance :
    SagaStateMachineInstance
{
    public SagaInstance(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    protected SagaInstance()
    {
    }

    public string CurrentState { get; set; }
    public string ServiceName { get; set; }
    public Guid CorrelationId { get; set; }
}
{% endhighlight %}

The saga repository is created using the simple syntax:

{% highlight C# %}
var database = new MongoClient("mongodb://127.0.0.1").GetDatabase("sagas");
var repository = new MongoDbSagaRepository<SagaInstance>(database);
{% endhighlight %}

Each saga instance will be placed in a collection specific to the instance type.


#### NHibernate

While the project seems dead, NHibernate is still widely used and is supported by MassTransit for saga storage. The example below shows the code-first approach to using NHibernate for saga persistence.

{% highlight C# %}
public class SagaInstance :
    SagaStateMachineInstance
{
    public SagaInstance(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    protected SagaInstance()
    {
    }

    public string CurrentState { get; set; }
    public string ServiceName { get; set; }
    public Guid CorrelationId { get; set; }
}


public class SagaInstanceMap :
    SagaClassMapping<SagaInstance>
{
    public SagaInstanceMap()
    {
        Property(x => x.CurrentState);
        Property(x => x.ServiceName, x => x.Length(40));
    }
}
{% endhighlight %}

The `SagaClassMapping` base class maps the `CorrelationId` of the saga, and handles some of the basic bootstrapping of the class map. All of the properties, including the property for the `CurrentState` (if you're using state machine sagas), must be mapped by the developer. Once mapped, the `ISessionFactory` can be created using NHibernate directly. From the session factory, the saga repository can be created.

{% highlight C# %}
ISessionFactory sessionFactory = CreateSessionFactory();
var repository = new NHibernateSagaRepository<SagaInstance>(sessionFactory);
{% endhighlight %}
