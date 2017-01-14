# Transports

This pages shortly describes the included transports.
It is work in progress and more content will be coming soon.

## RabbitMQ configuration options

_This page has not been updated yet._

This is the recommended approach for configuring MassTransit for use with RabbitMQ.

```csharp
Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host(new Uri("rabbitmq://a-machine-name/a-virtual-host"), host =>
    {
        host.Username("username");
        host.Password("password");
    });
});
```

## Azure Service Bus configuration options

```csharp
Bus.Factory.CreateUsingAzureServiceBus(cfg =>
{
    cfg.Host(new Uri("sb://localhost"), host =>
    {
        host.OperationTimeout = TimeSpan.FromSeconds(5);
        host.TokenProvider = new ????();
    });
});
```

About Azure Service Bus

## In-Memory transport

<div class="alert alert-warning">
<b>Note:</b>
    The in-memory transport is designed for use within a single process only.
    It is not possible to use the in-memory transport to communicate between multiple processes
    (even if they are on the same machine). By the way, it is possible to share the same
    in-memory transport with multiple bus instances *within the same process* by configuring
    the transport provider using InMemoryTransportCache (see below).
</div>

<div class="alert alert-warning">
<b>Note:</b>
    The InMemory transport is a great tool for testing, as it doesn't require a message broker
    to be installed or running. It's also very fast. But it isn't durable, and messages are gone
    if the bus is stopped or the process terminates. So, it's generally not a smart option for a
    production system. However, there are places where durability it not important so the cautionary
    tale is to proceed with caution.
</div>

The In-Memory transport uses the `loopback` address (a holdover from previous version of MassTransit).
The host doesn't matter, and the queue_name is the name of the queue.

    `loopback://localhost/queue_name`

```csharp
var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("queue_name", ep =>
    {
        //configure the endpoint
    })
});
```

### Sharing transports

While it seems weird, and again, it's probably only useful in test scenarios, the transport cache
can be shared across bus instances. To share a transport cache, use the syntax below.

```csharp
var inMemoryTransportCache = new InMemoryTransportCache(Environment.ProcessorCount);

var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.SetTransportProvider(inMemoryTransportCache);
});
```

As many bus instances as desired can share the same cache. Again, useful for testing. Not sure I'd
want to use this anywhere else.
