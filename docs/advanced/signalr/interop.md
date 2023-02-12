# Interop

The nice thing about using MassTransit as the back end is we can interact with the backplane by publishing the appropriate message (with hub).

 I can't think of a scenario you would ever publish `GroupManagement<THub>`. Only `All<THub>`, `Connection<THub>`, `Group<THub>`, and `User<THub>` should be used.

To publish a message from a back end service (eg. console app, Topshelf):

```csharp
await busControl.Publish<All<ChatHub>>(new
{
    Messages = protocols.ToProtocolDictionary("broadcastMessage", new object[] { "backend-process", "Hello" })
});
```

You are done!

The lifetime of the message looks like this:

1. An `All<ChatHub>` message, including your payload will be published to your configured broker.
2. All (or a select few, depending on how you configured MassTransit) consumers which registered `.AddSignalRHub<ChatHub>(...)` will receive the message.
3. The message will be published through the SignalR connection.
4. Your client will receive the message.

A slightly more complex example of how you would send a message to a specific group, but exclude specific connection ids would look like this:

```csharp
await busControl.Publish<Group<ChatHub>>(new
{
	GroupName = "ServiceDeskEmployees",
	ExcludedConnectionIds = new [] { "11b9c749-69a2-4f3e-8a8b-968122156220", "1737778b-c836-4023-a255-51c2e4898c43" },
    Messages = protocols.ToProtocolDictionary("broadcastMessage", new object[] { "backend-process", "Hello" })
});
```

This example would send the message to a group called "ServiceDeskEmployees", but exclude the specified connection ids from receiving the message.

## Complex Hubs

Your ASP.NET Core might have complex Hubs, with multiple interfaces injected.

```csharp
public class ProductHub : Hub
{
    public ProductHub(
        IService1 service1,
        IService2 service2,
        ICache cache,
        IMapper mapper
    )
    {
        //...
    }

    // Hub Methods...
}
```

Your back end service might exist in a separate project and namespace, with no knowledge of the hubs or injected services. However, even if said service does not use SignalR, you might still want to publish messages which pass through the broker and end up being sent to your client.

 Because MassTransit routes messages by namespace+message, I recommend to create a marker hub(s) within your back end service just for use of publishing. This saves you having to have all the hub(s) injected dependencies also within your back end service and still allows your service to publish namespace-compliant messages to be picked up by your SignalR integration h

```csharp
namespace YourNamespace.Should.Match.The.Hubs
{
    public class ProductHub : Hub
    {
        // That's it, nothing more needed.
    }
}
```

## Protocol Dictionary

SignalR supports multiple protocols for communicating with the Hub, the "serialized message" that is sent over the backplane is translated for each protocol method supported. The Extension method `.ToProtocolDictionary(...)` helps facilitate this translation into the protocol for communication.
