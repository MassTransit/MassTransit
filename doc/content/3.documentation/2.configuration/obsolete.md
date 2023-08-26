---
navigation.title: Obsolete
---

# Obsolete Methods

MassTransit retains a lot of older methods to avoid breaking builds when upgrading to new versions. While these methods should still work as they did previously,
they don't represent the current usage guidelines.

> Since obsolete method warnings can be ignored, they have been added to many methods that are completely usable but not recommended.

## UseRetry

The `UseRetry` middleware method was replaced by `UseMessageRetry`. The only scenario where the previous method should be used is in specific message
pipelines where retry within the same consumer or saga context is preferred. 

## Consumer Definitions

The _ConfigureConsumer_, _ConfigureSaga_, _ConfigureExecuteActivity_, and _ConfigureCompensateActivity_ methods in consumer, saga, and activity definitions
have a new overload that adds the `IRegistrationContext` parameter.

> The `IRegistrationContext` interface implements `IServiceProvider` as well, so there is no need to inject it into the constructor. 

The `IRegistrationContext` argument should be passed to methods that accept it, or that may have previously accepted `IServiceProvider`, to ensure the correct
context is used for the bus and to ensure message scope is properly handled.

For example, the outbox now requires `IRegistrationContext` as an argument.

```csharp
protected override void ConfigureSaga(IReceiveEndpointConfigurator configurator, 
    ISagaConfigurator<JobAttemptSaga> sagaConfigurator,
    IRegistrationContext context)
{
    configurator.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));

    // use the new overload, not the obsolete one
    configurator.UseInMemoryOutbox(context);
}
```

## AddMassTransitInMemoryTestHarness

The original test harness has been deprecated, use `AddMassTransitTestHarness` instead. The current test harness is covered in the
[Testing](/documentation/concepts/testing) documentation. Additional methods including `AddConsumerTestHarness`, `AddSagaTestHarness`, 
and `AddSagaStateMachineTestHarness` are not used with the new test harness. 

## AddBus

Transports have their own `UsingXxx` methods, such as `UsingRabbitMq`. To ensure the container and all supporting bus components are properly configured, 
the `UsingXxx` methods should be used.

## Instance, Data (Saga State Machines)

The _Automatonymous_ properties `Instance` and `Data` have been superseded by `Saga` and `Message` to be consistent with MassTransit terminology.

## Job Service Configuration

The job service configuration was drastically simplified in v8.1 - eliminating the need to configure the service instance. 
See the [updated documentation](/documentation/patterns/job-consumers) for details.
