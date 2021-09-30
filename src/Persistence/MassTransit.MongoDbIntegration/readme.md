# MassTransit.MongoDbIntegration

This package supports MongoDb, including the storage of sagas, message data, and routing slip events in MongoDB. For sagas, this means that you can orchestrate a long-running process and manage its state throughout execution. For message data, you can use GridFS to store large files. For routing slips, you can store the events produced by a routing slip as documents.

## Attribution

Many parts of this package, including the message data and saga repositories, were originally created by [Liberis Labs](http://goo.gl/VaFgpk), including [Lee Blundell](https://github.com/Blundell89), [Kevin Smith](https://github.com/kevbite), and others. The original packages were [here](https://github.com/LiberisLabs/MassTransit.MessageData.MongoDb) and [here](https://github.com/LiberisLabs/MassTransit.Persistence.MongoDb). They were copied with permission from the authors into this official assembly.

## Getting Started

MassTransit.MongoDbIntegration can be installed via the package manager console by executing the following commandlet:

```powershell
PM> Install-Package MassTransit.MongoDbIntegration
```

## Saga Storage

Once we have the package installed, we can create a `MongoDbSagaRepository` using one of the following constructors:

```csharp
var repository = new MongoDbSagaRepository(new MongoUrl("mongodb://localhost/masstransitTest"));
```

Or

```csharp
var repository = new MongoDbSagaRepository("mongodb://localhost", "masstransitTest"));
```

## Initiating a Simple Saga

Say we have an `InitiateSimpleSaga` message:

```csharp
class InitiateSimpleSaga :
        CorrelatedBy<Guid>
    {
        public InitiateSimpleSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }
    }
```

And we have a `SimpleSaga` that is initiated by the `InitiateSimpleSaga` message:

```csharp
class SimpleSaga :
        InitiatedBy<InitiateSimpleSaga>,
        IVersionedSaga
    {
        public Guid CorrelationId { get; set; }

        public int Version { get; set; }

        public Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            //Do some cool stuff...
            Console.WriteLine($"{nameof(InitiateSimpleSaga)} consumed");

            return Task.FromResult(0);
        }
    }

```

Now we have our saga and our initiation message, we can set up our bus to use the `MongoDbSagaRepository`:

```csharp
var busControl = Bus.Factory.CreateUsingInMemory(configurator =>
    {
        configurator.ReceiveEndpoint("my_awesome_endpoint", endpoint =>
        {
            //Normal receive endpoint config...

            endpoint.Saga(new MongoDbSagaRepository<SimpleSaga>(new MongoUrl("mongodb://localhost/masstransitTest")));
        });
    });
```

With everything now configured we can raise the `InitiateSimpleSaga` message to get the saga kicked off:

```csharp
var id = Guid.NewGuid();

var message = new InitiateSimpleSaga(id);

await busControl.Publish(message);
```


## Message Data 

This package allows big data messages to be stored in MongoDB, this stops your messaging system getting clogged up with big payloads.

Once we have the package installed, we can create a `MongoMessageDataRepository` using one of the following constructors:

```csharp

var repository = new MongoMessageDataRepository(new MongoUrl("mongodb://localhost/masstransitTest"));

```

Or

```csharp

var repository = new MongoMessageDataRepository("mongodb://localhost", "masstransitTest");

```

#### Sending a Big Message

Say we have a `BigMessage` that has a  `BigPayload` property of a type of `MessageData<byte[]>`:

```csharp

public class BigMessage
{
    public string SomeProperty1 { get; set; }

    public int SomeProperty2 { get; set; }

    public MessageData<byte[]> BigPayload { get; set; }
}

```

When we create the message we need to call our `MongoMessageDataRepository` to put the big payload into MongoDB, which in turn passes back a `MessageData<byte[]>`:

```csharp
var blob = new byte[] {111, 2, 234, 12, 99};

var bigPayload = await repository.PutBytes(blob);

var message = new BigMessage
{
    SomeProperty1 = "Other property that will get passed on message queue",
    SomeProperty2 = 12,
    BigPayload =  bigPayload
};

```

We can then publish/send it like any other MassTransit message:

```csharp

busControl.Publish(message);

```

#### Receiving a Big Message

To receive a message with a big payload we need to configure our endpoint to use the repository for a given message type:

```csharp

var busControl = MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("my_awesome_endpoint", ep =>
    {
        // Normal Receive Endpoint Config...

        ep.UseMessageData<BigMessage>(repository);
    });
});

```

Then, with the magic wiring from MassTransit we can consume the message inside a consumer with the following:
```csharp

public class BigMessageConsumer : IConsumer<BigMessage>
{
    public async Task Consume(ConsumeContext<BigMessage> context)
    {
        var bigPayload = await context.Message.BigPayload.Value;

        // Do something with the big payload...
    }
}

```

## Cleaning up Expired GridFS Data

Expired GridFS data can be removed by running the following script:

```javascript
// deleteExpiredMessageData.js

var docs = db.getMongo().getDB("masstransit");
var now = new Date().toISOString();

var cursor = docs.fs.files.find({"metadata.expiration" : {$lte : new Date(now)}});

cursor.forEach(function (toDelete) {
    var id = toDelete._id;
    docs.fs.chunks.remove({files_id : id});
    docs.fs.files.remove({_id : id});
});    
```

Or if you wish to impliment in C# e.g. via a hosted service

```csharp
MongoClient client = new MongoClient(connectionString);
IMongoDatabase database = client.GetDatabase(databaseName);
var bucket = new GridFSBucket(database);

DateTime now = DateTime.UtcNow;

var filter = Builders<GridFSFileInfo>.Filter.Lte(x => x.Metadata["expiration"], now);
var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.Length);
var options = new GridFSFindOptions
{
    Sort = sort
};

using (var cursor = await bucket.FindAsync(filter, options))
{
    await cursor.ForEachAsync(async file =>
    {
        await bucket.DeleteAsync(file.Id);
    });
}
```

Alternatively, you can import the `CreateDeleteExpiredMassTransitMessageDataTask.xml` file into Windows Task Scheduler and configure the script's arguments so that expired documents are deleted on a schedule.

# Contribute
1. Fork
2. Hack!
3. Pull Request
