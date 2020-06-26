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

Your back end service might exist in a separate project and namespace, with no knowledge of the hubs or injected services. Because MassTransit routes messages by namespace+message, I recommend to create a marker hub(s) within your back end service just for use of publishing. This saves you having to have all the hub(s) injected dependencies also within your back end service.

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
