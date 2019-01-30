# Creating Automatonymous State Machines

[Automatonymous][1] is a state machine library built by the same team that created MassTransit. Automatonymous provides a friendly syntax for declaring a state machine, including the states, events (both trigger event and data events are supported), and behaviors. The simple syntax makes it easy to get started with your own state machines, while including many advanced features that make it extremely flexible in a variety of business contexts.

Like MassTransit, Automatonymous is free, open source, and licensed under the very permissive Apache 2.0 license, making usable at no cost to anyone for both commercial and non-commercial use.

## Sample State Machine

The code in this section will require installing a few NuGet dependencies:

* `MassTransit`
* `Automatonymous`
* `MassTransit.Automatonymous`

### Defining a state machine saga

To define a state machine saga, create a class that inherits from `MassTransitStateMachine`. The `MassTransit.Automatonymous` NuGet package must be referenced.

> This section is written using the shopping cart sample, which is [hosted on GitHub][2].

```csharp
public class ShoppingCartStateMachine :
    MassTransitStateMachine<ShoppingCart>
{
    public ShoppingCartStateMachine()
    {
        InstanceState(x => x.CurrentState);
```

The state machine class, and the specification of the property for the current state of the machine are defined first.

```csharp
    Event(() => ItemAdded, x => x.CorrelateBy(cart => cart.UserName, context => context.Message.UserName)
        .SelectId(context => Guid.NewGuid()));
```

The event that is observed when an item is added to the cart, along with the correlation between the state machine instance and the message are defined. The id generator for the saga instance is also defined.

```csharp
    Event(() => Submitted, x => x.CorrelateById(context => context.Message.CartId));
```

The order submitted event, and the correlation for that order.

```csharp
    Schedule(() => CartExpired, x => x.ExpirationId, x =>
    {
        x.Delay = TimeSpan.FromSeconds(10);
        x.Received = e => e.CorrelateById(context => context.Message.CartId);
    });
```

In order to schedule the timeout, a schedule is defined, including the time delay for the scheduled event, and the correlation of the event back to the state machine.

Now, it is time for the actual behavior of the events and how they interact with the state of the *ShoppingCart*.

```csharp
    Initially(
        When(ItemAdded)
            .Then(context =>
            {
                context.Instance.Created = context.Data.Timestamp;
                context.Instance.Updated = context.Data.Timestamp;
                context.Instance.UserName = context.Data.UserName;
            })
            .ThenAsync(context => 
                Console.Out.WriteLineAsync(
                    $"Item Added: {context.Data.UserName} to {context.Instance.CorrelationId}"))
            .Schedule(CartExpired, context => new CartExpiredEvent(context.Instance))
            .TransitionTo(Active)
        );
```

Initially defined events that can create a state machine instance. In the above, the properties of the instance are initialized, and then the *CartExpired* event is scheduled, after which the state is set to *Active*.

```csharp
    During(Active,
        When(Submitted)
            .Then(context =>
            {
                if (context.Data.Timestamp > context.Instance.Updated)
                    context.Instance.Updated = context.Data.Timestamp;
                context.Instance.OrderId = context.Data.OrderId;
            })
            .ThenAsync(context => 
                Console.Out.WriteLineAsync(
                    $"Cart Submitted: {context.Data.UserName} to {context.Instance.CorrelationId}"))
            .Unschedule(CartExpired)
            .TransitionTo(Ordered),
```

While the shopping cart is active, if the order is submitted, the expiration is canceled (via *Unschedule*) and the state is set to Ordered.

```csharp
            When(ItemAdded)
                .Then(context =>
                {
                    if (context.Data.Timestamp > context.Instance.Updated)
                        context.Instance.Updated = context.Data.Timestamp;
                })
                .ThenAsync(context => 
                    Console.Out.WriteLineAsync(
                        $"Item Added: {context.Data.UserName} to {context.Instance.CorrelationId}"))
                .Schedule(CartExpired, context => new CartExpiredEvent(context.Instance)),
```

If another item is added to the cart, the *CartExpired* event is scheduled, and the existence of a previously scheduled event (via the *ExpirationId* property) is used to cancel the previously scheduled event.

```csharp
        When(CartExpired.Received)
            .ThenAsync(context => Console.Out.WriteLineAsync(
                $"Item Expired: {context.Instance.CorrelationId}"))
            .Publish(context => new CartRemovedEvent(context.Instance))
            .Finalize()
        );
```

If the *CartExpired* event is received, the cart removed event is published and the shopping cart is finalized.

```csharp
        SetCompletedWhenFinalized();
    }
```

Signals that the state machine instance should be deleted if it is finalized. This is used to tell Entity Framework to delete the row from the database.

```csharp
    public State Active { get; private set; }
    public State Ordered { get; private set; }
```

The states of the shopping cart (*Initial* and *Final* are built-in states).

```csharp
    public Schedule<ShoppingCart, CartExpired> CartExpired { get; private set; }
```

The schedule definition for the CartExpired event.

```csharp
    public Event<CartItemAdded> ItemAdded { get; private set; }
    public Event<OrderSubmitted> Submitted { get; private set; }
}
```

The events that are observed by the state machine (the correlations are defined earlier in the state machine).

The state machine is generic, and requires a state class (because sagas are stateful), so that is defined below. The state class has the values that are persisted between events.

```csharp
class ShoppingCartState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
```

The CorrelationId is the primary key of the saga state instance. It is assigned either from a property on the initial message that creates the saga instance, or can be generated using `NewId.NextGuid()`, which ensures a nice ordered sequential identifier.

```csharp
    public string CurrentState { get; set; }
```

The current state of the saga, which can be saved as a *string* or an *int*, depending upon your database requirements. An *int* is smaller, but requires that all valid states be mapped to integers during the definition of the state machine.

```csharp
    public Guid? ExpirationId { get; set; }
```

This is an identifier that is used by the state machine's scheduling feature, to capture the scheduled message identifier. Message scheduling within sagas is a powerful feature, which is described later.

```csharp
    public string UserName { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public Guid? OrderId { get; set; }
}
```

The remainder of the properties are relevant to the application, and are saved when properly mapped using the saga repository (which can be any supported storage engine, Entity Framework and NHibernate are supported out of the box).

### Connecting the saga to a receive endpoint

To connect the state machine saga to a receive endpoint, a saga repository is used, along with the state machine instance.

```csharp
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
```

### Combining events (think Fork/Join)

Multiple events can be combined into a single event, for the purposes of joining together multiple operations. To define a combined event, the `Event` syntax has an overload.

```csharp
    public Event<OrderReady> Ready { get; private set; }
    public Event<PaymentApproved> Approved { get; private set; }
    public Event<StockVerified> Verified { get; private set; }

    CompositeEvent(() => OrderReady, x => x.OrderReadyStatus, PaymentApproved, StockVerified);
```

Once both events have been delivered to the state machine, the third event, *OrderReady*, will be triggered.

<div class="alert alert-info">
<b>Note:</b>
    The order of events being declared can impact the order in which they execute. Therefore, it is best to declare composite events at the end of the state machine declaration, after all other events and behaviors are declared. That way, the composite events will be raised <i>after</i> the dependent event behaviors.
</div>


### Ignoring events

In some cases you may want to ignore events, which can not be handled by the saga, because of inapropriate state. Without additional configuration handling of such events end with exception and messages being moved to the error queue. State machine can be configured to ignore events if they can not be handled. This can be done with the use of <i>Ignore()</i> method:

```csharp
    public State Intermediate { get; set; }
    public Event Start { get; private set; }

    public MyMachine()
    {
        Initially(
            When(Start)
                .ThenAsync(context => Console.Out.WriteAsync("Initially"))
                .TransitionTo(Intermediate)
            );

        During(Intermediate,
            When(AllowedEvent)
                .Then(context => Console.Out.WriteAsync("AllowedEvent"))
                .TransitionTo(SomeState),
            Ignore(Start)
            );
    }
```

In the example above MyMachine will only handle Start event in <i>Initial</i> state. If Start event is correlated with the saga, which is in the state <i>Intermediate</i>, such event will be ignored without moving message to the error queue.

Another approach to configure ignorance is to use <i>Ignore()</i> method inside <i>DuringAny()</i>. This approach is more suitable when a sufficiently large number of states is defined:

```csharp
    public State Intermediate { get; set; }
    public State AnotherState { get; set; }
    public State SomeAnotherState { get; set; }
    public Event Start { get; private set; }

    public MyMachine()
    {
        DuringAny(Ignore(Start));

        Initially(
            When(Start)
                .ThenAsync(context => Console.Out.WriteAsync("Initially"))
                .TransitionTo(Intermediate)
            );

        During(Intermediate,
            When(AllowedEvent)
                .Then(context => Console.Out.WriteAsync("AllowedEvent"))
            Ignore(Start)
            );
    }
```

In the example above MyMachine is configured to ignore <i>Start</i> event during any state, except <i>Initial</i> state.

[1]: https://github.com/MassTransit/Automatonymous
[2]: https://github.com/MassTransit/Sample-ShoppingWeb