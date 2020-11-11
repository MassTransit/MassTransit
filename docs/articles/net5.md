# Moving to .NET 5

Microsoft has [released](https://devblogs.microsoft.com/dotnet/announcing-net-5-0/) .NET 5, which can be [downloaded](https://dotnet.microsoft.com/download/dotnet/5.0) now. There are significant new features available while maintaining compatibility with previous versions (including the LTS 3.1 release, which will continue to be supported).

This release also includes [C# 9](https://devblogs.microsoft.com/dotnet/c-9-0-on-the-record/), which has a bunch of new language features – one of the most important being _records_.

This article summarizes some of the new features and how they relate to MassTransit, along with some general thoughts. These are just what has been found so far, there are surely more useful applications of new runtime and language features.

## Records

One of the coolest new features in .NET 5, a record is a read-only (immutable) data structure. A record is a reference type, which makes it a great message type. For example, consider the following message contract.

```cs
public interface OrderSubmitted
{
    string OrderId { get; }
    DateTime OrderDate { get; }
}
```

To publish the _OrderSubmitted_ event, a message initializer is used to create the event.

```cs
bus.Publish<OrderSubmitted>(new { OrderId = "46", OrderDate = DateTime.UtcNow });
```

This calls the _Publish_ overload with the following signature:

```cs
Task Publish<T>(object values, CancellationToken cancellationToken)
```

Using the new record type, this contract could be rewritten as shown below.

```cs
public record OrderSubmitted
{
    public string OrderId { get; init; }
    public DateTime OrderDate { get; init; }
}
```

The record type event could be published the same way, using a message initializer, as shown above. Another way would be to use the record initializer to send the actual message type created without using a message initializer.

```cs
bus.Publish<OrderSubmitted>(new() { OrderId = "46", OrderDate = DateTime.UtcNow });
```

Did you spot the difference? It's subtle. C# 9 includes "target-typing", where the target type is known for the _new_ expression. This means that the type no longer needs to be specified when using _new_. The `()` is the only difference, which creates a specific instance of the record type instead of an anonymous type (which is passed to the message initializer). This in turn calls a different _Publish_ overload.

```cs
Task Publish<T>(T message, CancellationToken cancellationToken)
```

::: tip NOTE
When using record type initializers, message type initializers, along with type conversion and shortcut variables (via `InVar`) are not available.
:::

Since a record is a reference type, and under the covers a record has private setters for properties, they serialize as expected. If record constructors are used, it may be necessary to include a default constructor to support proper deserialization.

## Module Initializers

I recently commented during one of the [Season 2](https://www.youtube.com/playlist?list=PLx8uyNNs1ri1UA_Nerr7Ej3g9nT2PxbbH) episodes on YouTube that I wished C# had module initializers. Well, sure as s--t, they're now part of C# 9. And one of the other clever tricks is the ability to include a method in an interface, in this case, a static internal method, that is marked with the `[ModuleInitializer]` attribute. In this method, MassTransit's global topology is being used to configure the `CorrelationId` for the message contract.

```cs
public interface OrderSubmitted
{
    Guid OrderId { get; }
    DateTime OrderDate { get; }

    [ModuleInitializer]
    internal static void Init()
    {
        GlobalTopology.Send.UseCorrelationId<OrderSubmitted>(x => x.OrderId);
    }
}
```

This method will be called automatically by the runtime, yet not be visible in the interface. This assumes that message contracts are in a separate assembly from consumers and producers.






