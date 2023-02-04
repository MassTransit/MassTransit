# Testing

MassTransit is a highly asynchronous framework that builds on top of `Microsoft.Extensions.DependencyInjection` which enables high performance and flexibly message solutions. However, those choices can make unit testing more complex that your average test. To make it reduce this burden, MassTransit includes a [Test Harnesses](/documentation/configuration/test-harness) which enables a testing platform that runs entirely in memory.  

Since MassTransit is typically configured using `AddMassTransit` in combination with `Microsoft.Extensions.DependencyInjection` the test harness will override your existing MassTransit configuration with in-memory options suitable for testing.

## Features

- Swaps `IBus` instances in place simplifying test configuration
- Async in nature
- Works with all dotnet testing frameworks

## High Level Concepts

Because MassTransit is an asynchronous framework, and its possible that a user may want to test the consumption of a message as well as any events that are being published as well, the test harness uses multiple timers to assist in coordinating the various assertions. Because of this, we need to create a new `TestHarness` instance for each test. It is therefore important that you not reuse an instance across a test. This means that you will have to create a new instance for each test.

```csharp
[Test] 
public async Task ASampleTest() 
{
    await using var provider = new ServiceCollection()
        // register all of your normal business services
        .AddYourBusinessServices()
        // AddMassTransitTestHarness will override your 
        // transport specific configuration with an in-memory
        // option
        .AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumer<SubmitOrderConsumer>();
        })
        .BuildServiceProvider(true);

    var harness = provider.GetRequiredService<ITestHarness>();

    // you only want to call Start once
    await harness.Start();

    var client = harness.GetRequestClient<SubmitOrder>();

    var response = await client.GetResponse<OrderSubmitted>(new
    {
        OrderId = InVar.Id,
        OrderNumber = "123"
    });

    Assert.IsTrue(await harness.Sent.Any<OrderSubmitted>());

    Assert.IsTrue(await harness.Consumed.Any<SubmitOrder>());

    var consumerHarness = harness.GetConsumerHarness<SubmitOrderConsumer>();

    Assert.That(await consumerHarness.Consumed.Any<SubmitOrder>());

    // test side effects of the SubmitOrderConsumer here
}
```

## Examples

The following are examples of using the `TestHarness` to test various components.

### Consumer 

To test a consumer using the MassTransit Test Harness:

```csharp
[Test]
public async Task ASampleTest() 
{
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

    // test side effects of the SubmitOrderConsumer here
}
```

### Saga State Machine

To test a saga state machine using the MassTransit Test Harness:

```csharp
[Test]
public async Task ASampleTest()
{
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

    var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, sagaHarness.StateMachine.Submitted);
    Assert.IsNotNull(instance, "Saga instance not found");
    Assert.That(instance.OrderNumber, Is.EqualTo(orderNumber));

    Assert.IsTrue(await harness.Published.Any<OrderApprovalRequired>());

    // test side effects of OrderState here
}
```
