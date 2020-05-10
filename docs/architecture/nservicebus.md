# NServiceBus

[NServiceBus](https://particular.net/nservicebus) is a commercial .NET messaging and workflow solution. 

### Interoperability

- [MassTransit.Interop.NServiceBus](https://nuget.org/packages/MassTransit.Interop.NServiceBus/)

::: danger
This package was built using a black box, clean room approach based on observed message formats within the message broker. As such, there may be edge cases and situations which are not handled by this package. Extensive testing is recommended to ensure all message properties are being properly interpreted.
:::

MassTransit has limited message exchange support with NServiceBus, tested with the formats and configurations listed below.

### Supported Serialization Formats

- JSON (Newtonsoft)
- XML

### Header Support

MassTransit delivers messages to consumers using `ConsumeContext<TMessage>`, which includes various header properties. From looking at the transport message format, headers are serialized outside of the message body. To support access to these headers in MassTransit, the transport headers are mapped as shown.

| Property | Header Used | Out | Notes
|:--------------|:-------------------------|:---:|:--------
| ContentType | NServiceBus.ContentType | Y
| ConversationId | NServiceBus.ConversationId | Y
| CorrelationId | NServiceBus.CorrelationId | Y
| MessageId | NServiceBus.MessageId | Y
| SourceAddress | NServiceBus.OriginatingEndpoint | Y | formatted as `queue:name`
| ResponseAddress | NServiceBus.ReplyToAddress | Y | formatted as `queue:name`
| SentTime | NServiceBus.TimeSent | Y
| Host.MachineName | NServiceBus.OriginatingMachine | Y
| Host.MassTransitVersion | NServiceBus.Version | N | translated to NServiceBus x.x.x
| Supported Message Types | NServiceBus.EnclosedMessageTypes | Y | converted from AssemblyQualifiedName, types must be resolvable via Type.GetType()
|  | NServiceBus.MessageIntent | N | Ignored

### RabbitMQ

NServiceBus follows the same broker topology conventions established by MassTransit. This means exchange names should be the same, which means that publish/subsribe with receive endpoints should work as expected.

RabbitMQ was tested using the following NServiceBus configuration:

```cs
var endpointConfiguration = new EndpointConfiguration("Gateway.Producer");
endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
endpointConfiguration.EnableInstallers();

var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
transport.UseConventionalRoutingTopology();
transport.ConnectionString("host=localhost");
```

Two message contracts were created:

```cs
 public class ClockUpdated :
    IEvent
{
    public DateTime CurrentTime { get; set; }
}

public class ClockSynchronized :
    IEvent
{
    public string Host {get;set;}
}
```

From the NServiceBus endpoint, the `ClockUpdated` message was published:

```cs
var session = _provider.GetRequiredService<IMessageSession>();
await session.Publish(new ClockUpdated {CurrentTime = DateTime.UtcNow}, new PublishOptions());
```

And the handler in NServiceBus for the `ClockSynchronized` message:

```cs
public class ClockSynchronizedHandler :
    IHandleMessages<ClockSynchronized>
{
    readonly ILogger<ClockSynchronizedHandler> _logger;

    public ClockSynchronizedHandler(ILogger<ClockSynchronizedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ClockSynchronized message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Clock synchronized: {Host}", message.Host);

        return Task.CompletedTask;
    }
}
```

On the MassTransit side, the bus was configured to use the consumer with the same message contract assembly.

```cs
services.AddMassTransit(x =>
{
    x.AddConsumer<TimeConsumer>();

    x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.UseNServiceBusJsonSerializer();

        cfg.ConfigureEndpoints(context);
    }));
});
```

With the consumer:

```cs
class TimeConsumer :
    IConsumer<ClockUpdated>
{
    readonly ILogger<TimeConsumer> _logger;

    public TimeConsumer(ILogger<TimeConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ClockUpdated> context)
    {
        _logger.LogInformation("Clock was updated: {CurrentTime}", context.Message.CurrentTime);

        await context.Publish(new ClockSynchronized
        {
            Host = HostMetadataCache.Host.MachineName
        });
    }
}
```



