# Testing

Unit testing consumers, sagas, and routing slip activities is supported by using the test harnesses included with MassTransit. The harnesses can be used with any unit testing framework. There is also a _MassTransit.TestFramework_ assembly for use with [NUnit](https://nuget.org/packages/NuGet), which is used by MassTransit for its own unit tests.

## Test Harness

A test harness is provided that handles the entire setup, startup, and shutdown of a bus instance. Every transport has its own test harness, and each test harness is based upon the same core test harness.

To create the harness in a unit test, start, and then stop it:

```cs
public async Task Should_start_and_stop_the_test_harness()
{
    var harness = new InMemoryTestHarness();
    await harness.Start();

    await harness.Stop();
}
```

The example above is for the in-memory test harness, which is the fastest option for creating unit tests. Each test gets the benefit of having a complete end-to-end environment for test execution. This includes message serialization, consumer instantiation, asynchronous execution, basically all the things.

## Consumer

To test a consumer, using the consumer test harness (which can be used with any transport):

```cs
public async Task Should_test_the_consumer()
{
    var harness = new InMemoryTestHarness();
    var consumerHarness = harness.Consumer<MyConsumer>();

    await harness.Start();
    try
    {
        await harness.InputQueueSendEndpoint.Send(new MyMessage());

        // did the endpoint consume the message
        Assert.IsTrue(harness.Consumed.Select<MyMessage>().Any());

        // did the actual consumer consume the message
        Assert.IsTrue(consumerHarness.Consumed.Select<MyMessage>().Any());
    }
    finally
    {
        await harness.Stop();
    }
}
```

Adding the consumer harness to the test harness will configure the consumer on the receive endpoint. As messages are received and consumed, they are added to collections via observers so that test assertions can be used to verify message consumption.

If you want to test for consumer exceptions you can check for published `Fault<T>` messages

```cs
public async Task Should_test_the_consumer()
{
    var harness = new InMemoryTestHarness();
    var consumerHarness = harness.Consumer<MyConsumer>();

    await harness.Start();
    try
    {
        await harness.InputQueueSendEndpoint.Send(new MyMessage());

        // did the endpoint consume the message
        Assert.IsTrue(harness.Consumed.Select<MyMessage>().Any());

        // did the actual consumer consume the message
        Assert.IsTrue(consumerHarness.Consumed.Select<MyMessage>().Any());

        // did the actual consumer throws exception
        Assert.IsTrue(harness.Published.Select<Fault<MyMessage>>().Any();
    }
    finally
    {
        await harness.Stop();
    }
}
```

If the consumer published an event, that too could be observed:

```cs
class MyConsumer :
    IConsumer<MyMessage>
{
    public async Task Consume(ConsumeContext<MyMessage> context)
    {
        context.Publish(new MyEvent());
    }
}

public async Task Should_start_and_stop_the_test_harness()
{
    // ...

    // did the consumer publish the event
    Assert.IsTrue(harness.Published.Select<MyEvent>().Any());

    // ...
}
```

## Saga

A standard class-based saga can be testing using the saga test harness.

```cs
public async Task Should_test_the_saga()
{
    var harness = new InMemoryTestHarness();
    var sagaHarness = harness.Saga<MySaga>();

    await harness.Start();
    try
    {
        Guid sagaId = NewId.NextGuid();

        await harness.Publish(new InitiatingMessage(sagaId));

        // did the endpoint consume the message
        Assert.IsTrue(harness.Consumed.Select<InitiatingMessage>().Any());

        // did the actual consumer consume the message
        Assert.IsTrue(sagaHarness.Consumed.Select<InitiatingMessage>().Any());

        Assert.IsTrue(saga.Created.Select(x => x.CorrelationId == sagaId).Any());
    }
    finally
    {
        await harness.Stop();
    }
}
```


## State Machine Saga

```cs
public async Task Should_test_the_state_machine_saga()
{
    var machine = new MyStateMachine();

    var harness = new InMemoryTestHarness();
    var sagaHarness = harness.StateMachineSaga<MyInstance, MyStateMachine>(machine);

    await harness.Start();
    try
    {
        Guid sagaId = NewId.NextGuid();

        await harness.Bus.Publish(new InitialEvent(sagaId));

        // did the endpoint consume the message
        Assert.IsTrue(harness.Consumed.Select<InitialEvent>().Any());

        // did the actual consumer consume the message
        Assert.IsTrue(sagaHarness.Consumed.Select<InitialEvent>().Any());

        MyInstance instance = sagaHarness.Created.ContainsInState(sagaId, machine, machine.Active);
        Assert.IsNotNull(instance, "Saga instance not found");
    }
    finally
    {
        await harness.Stop();
    }
}
```
