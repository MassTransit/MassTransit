# Message Data

Message brokers are built to be fast, and when it comes to messages, _size matters_. In this case, however, bigger is not better — large messages negatively impact broker performance.

MassTransit offers a built-in solution which stores large data (either a string or a byte array) in a separate repository and replaces it with a reference to the stored data (yes, it's a URI, shocking I know) in the message body. When the message is consumed, the reference is replaced with the original data which is loaded from the repository.

Message data is an implementation of the [Claim Check](https://www.enterpriseintegrationpatterns.com/patterns/messaging/StoreInLibrary.html) pattern.

::: tip NOTE
The message producer and consumer must both have access to the message data repository.
:::

## Usage

To use message data, add a `MessageData<T>` property to a message. Properties can be anywhere in the message, nested within message properties, or in collections such as arrays, lists, or dictionaries.

```cs
public interface IndexDocumentContent
{
    Guid DocumentId { get; }
    MessageData<byte[]> Document { get; }
}
```

Then configure the receive endpoint.

```cs
IMessageDataRepository messageDataRepository = new InMemoryMessageDataRepository();

cfg.ReceiveEndpoint("document-service", e =>
{
    e.UseMessageData(messageDataRepository);

    e.Consumer<IndexDocumentConsumer>();
});
```

Configuring the message data middleware (via `UseMessageData`) adds a transform to replace any deserialized message data reference with an object that loads the message data asynchronously. By using middleware, the consumer doesn't need to use the message data repository. The consumer can simply use the property value to access the message data (asynchronously, of course). If the message data was not loaded, an exception will be thrown. The `HasValue` property is `true` if message data is present.

```cs
public class IndexDocumentConsumer :
    IConsumer<IndexDocumentContent>

public async Task Consume(ConsumeContext<IndexDocumentContent> context)
{
    byte[] document = await context.Message.Document.Value;
}
```

The producer stores the message data in the repository while creating the message. The message data expiration can be set by specifying a time-to-live when it is stored.

```cs
Guid documentId = NewId.NextGuid();
byte[] document = new byte[100000]; // get byte array, or a big string

await endpoint.Send<IndexDocumentContent>(new
{
    DocumentId = documentId,
    Document = repository.PutBytes(document, TimeSpan.FromDays(1))
});
```

The message data is stored, and the reference added to the outbound message.

::: tip
`PutBytes` is an asynchronous method and returns a `Task<MessageData<byte[]>>`. In this example, the object initializer syntax is used to create the outbound message. Since it knows the property type is `MessageData<byte[]>`, the initializer automatically gets the tasks result and initializes the property. Cool, eh?
:::

## Repositories

MassTransit includes several message data repositories.

| Name       | Description |
|:-----------|:------------|
| InMemoryMessageDataRepository | Entirely in memory, meant for unit testing
| FileSystemMessageDataRepository | Writes message data to the file system, which may be a network drive or other shared storage
| MongoDbMessageDataRepository | Stores message data using MongoDB's GridFS
| EncryptedMessageDataRepository | Adds encryption to any other message data repository

### File System

To configure the file system message data repository:

```cs
IMessageDataRepository CreateRepository(string path)
{
    var dataDirectory = new DirectoryInfo(path);

    return new FileSystemMessageDataRepository(dataDirectory);
}
```

### MongoDB

To configure the MongoDB GridFS message data repository, follow the example shown below.

```cs
IMessageDataRepository CreateRepository(string connectionString, string databaseName)
{
    return new MongoDbMessageDataRepository(connectionString, databaseName);
}
```









