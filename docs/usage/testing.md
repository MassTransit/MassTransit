# Testing

MassTransit is a framework, and follows the Hollywood principle – don't call us, we'll call you. This inversion of control, combined with asynchronous execution, can complicate unit tests. To make it easy, MassTransit includes test harnesses to create unit tests that run entirely in-memory but behave close to an actual message broker. In fact, the included memory-based messaging fabric was inspired by RabbitMQ exchanges and queues.

To create a unit test with the in-memory test harness, create and start the harness as shown.

> Uses [MassTransit](https://nuget.org/packages/MassTransit/),  [NUnit](https://nuget.org/packages/NUnit/)

To create the harness in a unit test, start, and then stop it:

<<< @/docs/code/testing/UsingInMemoryTestHarness.cs

The test harness encapsulates the bus configuration, providing various hooks to customize the bus  and receive endpoint configuration. The in-memory harness is the easiest to use, is lightning fast, and runs entirely in-memory without any external dependencies. Each setup and teardown of the harness provides a completed, end-to-end test environment. Messages sent, published, and consumed go through an entire lifecycle, including serialization, dispatch, deserialization, consumer instantiation, and asynchronous execution.

Each supported transport has its own test harness, built from a common base.

## Consumer

To test a consumer, use the consumer test harness (available with any transport).

<<< @/docs/code/testing/UsingInMemoryTestHarnessConsumer.cs

The harness configures the consumer on its receive endpoint. The test sends the _SubmitOrder_ event, and assertions are added using harness's observer collections.

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
