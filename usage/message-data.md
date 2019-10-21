# Message Data

Message brokers are built to be fast, and larger message sizes can affect broker performance. MassTransit offers an out of box option which will store a large payload (byte array) to a repository. Transmit the message over the broker with a Uri to the payload, and then load the payload back from the repository on the consumer.

First MassTransit offers a few repositories out of box.

* InMemoryMessageDataRepository
* FileSystemMessageDataRepository
* MongoDbMessageDataRepository
* EncryptedMessageDataRepository

For Receiving, you need to add the MessageData Middleware when constructing your Endpoints for Consumers.

Example with Microsoft DI:

```csharp
public class YourMessage
{
    public string YourName { get; set; }
    public MessageData<byte[]> TheLargeMessage { get; set; }
}
...
serviceCollection.AddSingleton(new InMemoryMessageDataRepository());
...
cfg.ReceiveEndpoint(host, "submit-order", e =>
{
    e.UseMessageData<YourMessage>(provider.GetService<IMessageDataRepository>());

    e.ConfigureConsumer<YourMessageConsumer>(provider);
});
```

Then when consuming the message, you need to await the retrieval of the message.

```csharp
public async Task Consume(ConsumeContext<YourMessage> context)
{
    var theLargeMessage = await context.Message.TheLargeMessage.Value;
    // Continue consumption
}
```

And before Sending a message, you need to put the item in the repository

```csharp
var message = new YourMessage
{
    YourName = "Test",
    TheLargeMessage = repository.PutBytes(yourByteArray) // repository is of the type IMessageDataRepository
};

// Then .Publish(), or .Send() your message as you normally would with MassTransit
```
