# NServiceBus

<iframe id="ytplayer" type="text/html" width="640" height="360"
  src="https://www.youtube.com/embed/jO4gJuuninA?autoplay=0">
</iframe>

[NServiceBus](https://particular.net/nservicebus) is a commercial .NET messaging and workflow solution. 


### Interoperability

This sample shows how to use the [MassTransit.Interop.NServiceBus](https://nuget.org/packages/MassTransit.Interop.NServiceBus/) package to send messages to an NServiceBus system. To see how to consume MassTransit messages by an NServiceBus system without using the interop package, see the [NServiceBus MassTransit Ingest Behavior docs](https://docs.particular.net/samples/pipeline/masstransit-messages/#nservicebus-subscriber-masstransit-ingest-behavior).

::alert{type="danger"}
This package was built using a black box, clean room approach based on observed message formats within the message broker. As such, there may be edge cases and situations which are not handled by this package. Extensive testing is recommended to ensure all message properties are being properly interpreted.
::

MassTransit has limited message exchange support with NServiceBus, tested with the formats and configurations listed below.

### Supported Serialization Formats

- JSON (Newtonsoft)
- XML

### Header Support

MassTransit delivers messages to consumers using `ConsumeContext<TMessage>`, which includes various header properties. From looking at the transport message format, headers are serialized outside of the message body. To support access to these headers in MassTransit, the transport headers are mapped as shown.

| Property                | Header Used                      | Out | Notes                                                                             |
|:------------------------|:---------------------------------|:---:|:----------------------------------------------------------------------------------|
| ContentType             | NServiceBus.ContentType          |  Y  |                                                                                   |
| ConversationId          | NServiceBus.ConversationId       |  Y  |                                                                                   |
| CorrelationId           | NServiceBus.CorrelationId        |  Y  |                                                                                   |
| MessageId               | NServiceBus.MessageId            |  Y  |                                                                                   |
| SourceAddress           | NServiceBus.OriginatingEndpoint  |  Y  | formatted as `queue:name`                                                         |
| ResponseAddress         | NServiceBus.ReplyToAddress       |  Y  | formatted as `queue:name`                                                         |
| SentTime                | NServiceBus.TimeSent             |  Y  |                                                                                   |
| Host.MachineName        | NServiceBus.OriginatingMachine   |  Y  |                                                                                   |
| Host.MassTransitVersion | NServiceBus.Version              |  N  | translated to NServiceBus x.x.x                                                   |
| Supported Message Types | NServiceBus.EnclosedMessageTypes |  Y  | converted from AssemblyQualifiedName, types must be resolvable via Type.GetType() |
|                         | NServiceBus.MessageIntent        |  N  | Ignored                                                                           |

### RabbitMQ

NServiceBus follows the same broker topology conventions established by MassTransit. This means exchange names should be the same, which means that publish/subsribe with receive endpoints should work as expected.

RabbitMQ was tested using the following NServiceBus configuration:

```csharp
var endpointConfiguration = new EndpointConfiguration("Gateway.Producer");
endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
endpointConfiguration.EnableInstallers();

var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
transport.UseConventionalRoutingTopology();
transport.ConnectionString("host=localhost");
```

Two message contracts were created:

```csharp
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

```csharp
var session = _provider.GetRequiredService<IMessageSession>();
await session.Publish(new ClockUpdated {CurrentTime = DateTime.UtcNow}, new PublishOptions());
```

And the handler in NServiceBus for the `ClockSynchronized` message:

```csharp
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

```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<TimeConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseNServiceBusJsonSerializer();

        cfg.ConfigureEndpoints(context);
    }));
});
```

With the consumer:

```csharp
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



