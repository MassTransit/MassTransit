# Middleware

MassTransit is built using a network of pipes and filters to dispatch messages. A pipe is composed of a series of filters, each of which is a key atom and are described below.

A detailed view of MassTransit's [Receive Pipeline](receive.md) is a good example of the sophistication possible.

Middleware components are configured using extension methods on any pipe configurator `IPipeConfigurator<T>`, and the extension methods all begin with `Use` to separate them from other methods.

To understand how middleware components are built, an understanding of filters and pipes is needed.

## Filters

A filter is a middleware component that performs a specific function, and should adhere to the single responsibility principal – do one thing, one thing only (and hopefully do it well). By sticking to this approach, developers are able to opt-in to each behavior without including unnecessary or unwatched functionality.

There are many filters included with GreenPipes, and a whole lot more of them are included with MassTransit. In fact, the entire MassTransit message flow is built around pipes and filters.

Developers can create their own filters. To create a filter, create a class that implements `IFilter<T>`.

```cs
public interface IFilter<T>
    where T : class, PipeContext
{
    void Probe(ProbeContext context);

    Task Send(T context, IPipe<T> next);
}
```

The _Probe_ method is used to interrogate the filter about its behavior. This should describe the filter in a way that a developer would understand its role when looking at a network graph. For example, a transaction filter may add the following to the context.

```cs
public void Probe(ProbeContext context)
{
    context.CreateFilterScope("transaction");
}
```

The _Send_ method is used to send contexts through the pipe to each filter. _Context_ is the actual context, and _next_ is used to pass the context to the next filter in the pipe. Send returns a Task, and should always follow the .NET guidelines for asynchronous methods.

### PipeContext

The _context_ type has a `PipeContext` constraint, which is another core atom in _GreenPipes_. A pipe context can include _payloads_, which are kept in a last-in, first-out (LIFO) collection. Payloads are identified by _type_, and can be retrieved, added, and updated using the `PipeContext` methods:

```cs
public interface PipeContext
{
    /// <summary>
    /// Used to cancel the execution of the context
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Checks if a payload is present in the context
    /// </summary>
    bool HasPayloadType(Type payloadType);

    /// <summary>
    /// Retrieves a payload from the pipe context
    /// </summary>
    /// <typeparam name="T">The payload type</typeparam>
    /// <param name="payload">The payload</param>
    /// <returns></returns>
    bool TryGetPayload<T>(out T payload)
        where T : class;

    /// <summary>
    /// Returns an existing payload or creates the payload using the factory method provided
    /// </summary>
    /// <typeparam name="T">The payload type</typeparam>
    /// <param name="payloadFactory">The payload factory is the payload is not present</param>
    /// <returns>The payload</returns>
    T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
        where T : class;

    /// <summary>
    /// Either adds a new payload, or updates an existing payload
    /// </summary>
    /// <param name="addFactory">The payload factory called if the payload is not present</param>
    /// <param name="updateFactory">The payload factory called if the payload already exists</param>
    /// <typeparam name="T">The payload type</typeparam>
    /// <returns></returns>
    T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        where T : class;
```

The payload methods are also used to check if a pipe context is another type of context. For example, to see if the `SendContext` is a `RabbitMqSendContext`, the `TryGetPayload` method should be used instead of trying to pattern match or cast the _context_ parameter.

```cs
public async Task Send(SendContext context, IPipe<SendContext> next)
{
    if(context.TryGetPayload<RabbitMqSendContext>(out var rabbitMqSendContext))
        rabbitMqSendContext.Priority = 3;

    return next.Send(context);
}
```

::: tip
It is entirely the filter's responsibility to call _Send_ on the _next_ parameter. This gives the filter ultimately control over the context and behavior. It is how the retry filter is able to retry – by controlling the context flow.
:::

User-defined payloads are easily added, so that subsequent filters can use them. The following example adds a payload.

```cs
public class SomePayload
{
    public int Value { get; set; }
}

public async Task Send(SendContext context, IPipe<SendContext> next)
{
    var payload = context.GetOrAddPayload(() => new SomePayload{Value = 27});

    return next.Send(context);
}
```

::: tip
Using interfaces for payload types is highly recommended.
:::

## Pipes

Filters are combined in sequence to form a pipe. A pipe configurator, along with a pipe builder, is used to configure and build a pipe.

```cs
public interface CustomContext :
    PipeContext
{
    string SomeThing { get; }
}

IPipe<CustomContext> pipe = Pipe.New<CustomContext>(x =>
{   
    x.UseFilter(new CustomFilter(...));
})
```

The `IPipe` interface is similar to `IFilter`, but a pipe hides the _next_ parameter as it is part of the pipe's structure. It is the pipe's responsibility to pass the
appropriate _next_ parameter to the individual filters in the pipe.

```cs
public interface IPipe<T>
    where T : class, PipeContext
{
    Task Send(T context);
}
```

Send can be called, passing a context instance as shown.

```cs
public class BaseCustomContext :
    BasePipeContext,
    CustomContext
{
    public string SomeThing { get; set; }
}

await pipe.Send(new BaseCustomContext { SomeThing = "Hello" });
```




