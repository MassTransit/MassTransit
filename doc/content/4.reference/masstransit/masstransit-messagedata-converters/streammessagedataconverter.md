---

title: StreamMessageDataConverter

---

# StreamMessageDataConverter

Namespace: MassTransit.MessageData.Converters

```csharp
public class StreamMessageDataConverter : IMessageDataConverter<Stream>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StreamMessageDataConverter](../masstransit-messagedata-converters/streammessagedataconverter)<br/>
Implements [IMessageDataConverter\<Stream\>](../masstransit-metadata/imessagedataconverter-1)

## Constructors

### **StreamMessageDataConverter()**

```csharp
public StreamMessageDataConverter()
```

## Methods

### **Convert(Stream, CancellationToken)**

```csharp
public Task<Stream> Convert(Stream stream, CancellationToken cancellationToken)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Stream\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
