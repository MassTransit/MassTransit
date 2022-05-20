# Testing

MassTransit is a framework, and follows the Hollywood principle – don't call us, we'll call you. This inversion of control, combined with asynchronous execution, can complicate unit tests. To make it easy, MassTransit includes test harnesses to create unit tests that run entirely in-memory but behave close to an actual message broker. In fact, the included memory-based messaging fabric was inspired by RabbitMQ exchanges and queues.

Since MassTransit is typically configured using `AddMassTransit`, the preferred testing approach is to use a `ServiceCollection` to configure the test combined with the test harness.

### Consumer 

To test a consumer using container-based configuration:

```cs
await using var provider = new ServiceCollection()
    .AddMassTransitTestHarness(cfg =>
    {
        cfg.AddConsumer<SubmitOrderConsumer>();
    })
    .BuildServiceProvider(true);

var harness = provider.GetRequiredService<ITestHarness>();

await harness.Start();

var client = harness.GetRequestClient<SubmitOrder>();

await client.GetResponse<OrderSubmitted>(new
{
    OrderId = InVar.Id,
    OrderNumber = "123"
});

Assert.IsTrue(await harness.Sent.Any<OrderSubmitted>());

Assert.IsTrue(await harness.Consumed.Any<SubmitOrder>());

var consumerHarness = harness.GetConsumerHarness<SubmitOrderConsumer>();

Assert.That(await consumerHarness.Consumed.Any<SubmitOrder>());
```

### Saga State Machine

To test a saga state machine using container-based configuration:

```cs
await using var provider = new ServiceCollection()
    .AddMassTransitTestHarness(cfg =>
    {
        cfg.AddSagaStateMachine<OrderStateMachine, OrderState>();
    })
    .BuildServiceProvider(true);

var harness = provider.GetRequiredService<ITestHarness>();

await harness.Start();

var sagaId = Guid.NewGuid();
var orderNumber = "ORDER123";

await harness.Bus.Publish(new OrderSubmitted
{
    CorrelationId = sagaId,
    OrderNumber = orderNumber
});

Assert.That(await harness.Consumed.Any<OrderSubmitted>());

var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, OrderState>();

Assert.That(await sagaHarness.Consumed.Any<OrderSubmitted>());

Assert.That(await sagaHarness.Created.Any(x => x.CorrelationId == sagaId));

var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, machine.Submitted);
Assert.IsNotNull(instance, "Saga instance not found");
Assert.That(instance.OrderNumber, Is.EqualTo(orderNumber));

Assert.IsTrue(await harness.Published.Any<OrderApprovalRequired>());
```
