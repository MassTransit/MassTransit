# Test Harness

The MassTransit [Test Harness](/documentation/concepts/testing) is a framework for extending your existing IOC registrations with a testable bus instance.

## Lifecycle Controlling methods

- `await harness.Start()` - start the timers, and capture of messages

## Communication Methods

- `harness.Bus.Publish(new MessageToPublish())`

## Instance Methods

- `Sent`: What messages were sent
- `Consumed`: What messages were consumed
- `Published`: What messages were published
