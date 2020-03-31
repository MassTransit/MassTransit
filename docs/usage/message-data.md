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

The bus must be configured to use message data, specifying the message data repository.

```cs
IMessageDataRepository messageDataRepository = new InMemoryMessageDataRepository();

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.UseMessageData(messageDataRepository);

    cfg.ReceiveEndpoint("document-service", e =>
    {
        e.Consumer<IndexDocumentConsumer>();
    });
});

```

::: tip
Previous versions of MassTransit required a generic type to be specified on the `UseMessageData` method. Individual receive endpoints could also be configured separately. The previous methods are deprecated and now a single bus configuration applies to all receive endpoints.
:::

Configuring the message data middleware (via `UseMessageData`) adds a transform to replace any deserialized message data reference with an object that loads the message data asynchronously. By using middleware, the consumer doesn't need to use the message data repository. The consumer can simply use the property value to access the message data (asynchronously, of course). If the message data was not loaded, an exception will be thrown. The `HasValue` property is `true` if message data is present.

```cs
public class IndexDocumentConsumer :
    IConsumer<IndexDocumentContent>

public async Task Consume(ConsumeContext<IndexDocumentContent> context)
{
    byte[] document = await context.Message.Document.Value;
}
```

To initialize a message contract with one or more `MessageData<T>` properties, the `byte[]` or `string` value can be specified and the data will be stored to the repository by the initializer. If the message has the _TimeToLive_ header property specified, that same value will be used for the message data in the repository. 

```cs
Guid documentId = NewId.NextGuid();
byte[] document = new byte[100000]; // get byte array, or a big string

await endpoint.Send<IndexDocumentContent>(new
{
    DocumentId = documentId,
    Document = document
});
```

If using a message class, or not using a message initializer, the data must be stored to the repository explictly.

```cs
class IndexDocumentContentMessage :
    IndexDocumentContent
{
    public Guid DocumentId { get; set; }
    public MessageData<byte[]> Document { get; set; }
}

Guid documentId = NewId.NextGuid();
byte[] document = new byte[100000]; // get byte array, or a big string

await endpoint.Send<IndexDocumentContent>(new IndexDocumentContentMessage
{
    DocumentId = documentId,
    Document = await repository.PutBytes(document, TimeSpan.FromDays(1))
});
```

The message data is stored, and the reference added to the outbound message.

## Configuration

There are several configuration settings available to adjust message data behavior.

### Time To Live

By default, there is no default message data time-to-live. To specify a default time-to-live, set the default as shown.

```cs
MessageDataDefaults.TimeToLive = TimeSpan.FromDays(2);
```

This settings simply specifies the default value when calling the repository, it is up to the repository to apply any time-to-live values to the actual message data.

If the `SendContext` has specified a time-to-live value, that value is applied to the message data automatically (when using message initializers). To add extra time, perhaps to account for system latency or differences in time, extra time can be added.

```cs
MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromMinutes(5);
```

### Inline Threshold

Newly added is the ability to specify a threshold for message data so that smaller values are included in the actual message body. This eliminates the need to read the data from storage, which increases performance. The message data can also be configured to _not_ write that data to the repository if it is under the threshold. By default (for now), data is written to the repository to support services that have not yet upgraded to the latest MassTransit.

> If you know your systems are upgraded, you can change the default so that data sizes under the threshold are not written to the repository.

To configure the threshold, and to optionally turn off storage of data sizes under the threshold:

```cs
// the default value is 4096
MessageDataDefaults.Threshold = 8192;

// to disable writing to the repository for sizes under the threshold
// defaults to false, which may change to true in a future release
MessageDataDefaults.AlwaysWriteToRepository = false;
```

## Repositories

MassTransit includes several message data repositories.

| Name       | Description |
|:-----------|:------------|
| InMemoryMessageDataRepository | Entirely in memory, meant for unit testing
| FileSystemMessageDataRepository | Writes message data to the file system, which may be a network drive or other shared storage
| MongoDbMessageDataRepository | Stores message data using MongoDB's GridFS
| AzureStorageMessageDataRepository | Stores message data using Azure Blob Storage
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

### Azure Storage

An Azure Cloud Storage account can be used to store message data. To configure Azure storage, first create the CloudStorageAccount object from your connection string, and then use the extension method to create the repository as shown below. You can replace `message-data` with the desired container name.

```cs
var account = CloudStorageAccount.Parse("<storage account connection string>");
_repository = account.CreateMessageDataRepository("message-data");
```
