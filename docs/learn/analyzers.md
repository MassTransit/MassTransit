# Roslyn Analyzers

MassTransit has a code analyzer which detects and provides code fixes which can be helpful to identify potential issues.

> Package: [MassTransit.Analyzers](https://www.nuget.org/packages/MassTransit.Analyzers)

## Message Initializers

[Message Initializers](/usage/producers.md##message-initializers) are used to initialize a message without having to create a backing class.

The analyzer supports methods that accept an `object` _values_ argument, including:

- `ISendEndpoint.Send<T>(object values)`
- `IPublishEndpoint.Publish<T>(object values)`
- `ConsumeContext.RespondAsync<T>(object values)`