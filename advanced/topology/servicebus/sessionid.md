# Azure Service Bus SessionId Convention

The SessionId on published/sent messages can be configured by convention, allowing the same method to be used for messages which implement a common interface type. If no common type is shared, each message type may be configured individually using various conventional selectors. Alternatively, developers may create their own convention to fit their needs.

When configuring a bus, the send topology can be used to specify a routing key formatter for a particular message type.

```csharp
public interface UpdateUserStatus
{
    Guid UserId { get; }
    string Status { get; }
}

Bus.Factory.CreateUsingAzureServiceBus(..., cfg =>
{
    cfg.Send<UpdateUserStatus>(x =>
    {
        x.UsePartitionKeyFormatter(context => context.Message.UserId);
    });
});
```
