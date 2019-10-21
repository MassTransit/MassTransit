# Transports

This pages shortly describes the included transports. It is work in progress and more content will be coming soon.

## RabbitMQ configuration options

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
Bus.Factory.CreateUsingAzureServiceBus(x =>
{
    var host = x.Host(serviceUri, h =>
    {
        h.SharedAccessSignature(s =>
        {
            s.KeyName = "keyName";
            s.SharedAccessKey = "key";
            s.TokenTimeToLive = TimeSpan.FromDays(1);
            s.TokenScope = TokenScope.Namespace;
        });
    });
});
```

## Amazon SQS

```csharp
Bus.Factory.CreateUsingAmazonSqs(x =>
{
    var host = x.Host(serviceUri, h =>
    {
        h.AccessKey(AccessKey);
        h.SecretKey(SecretKey);
    });
});
```

## ActiveMQ / AmazonMQ

```csharp
Bus.Factory.CreateUsingActiveMq(cfg =>
{
    var host = cfg.Host("{your-id}.mq.us-east-2.amazonaws.com", 61617, h =>
    {
        h.Username(TestUsername);
        h.Password(TestPassword);

        h.UseSsl();
    });
});
```

## In-Memory transport

<div class="alert alert-warning">
<b>Note:</b>
    The in-memory transport is designed for use within a single process only.
    It is not possible to use the in-memory transport to communicate between multiple processes (even if they are on the same machine).
</div>

<div class="alert alert-warning">
<b>Note:</b>
    The InMemory transport is a great tool for testing, as it doesn't require a message broker to be installed or running. It's also very fast. But it isn't durable, and messages are gone if the bus is stopped or the process terminates. So, it's generally not a smart option for a production system. However, there are places where durability it not important so the cautionary tale is to proceed with caution.
</div>

The In-Memory transport uses the `loopback` address (a holdover from previous version of MassTransit). The host doesn't matter, and the queue_name is the name of the queue.

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

