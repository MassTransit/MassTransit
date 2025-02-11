---

title: ByteArrayMessageDataConverter

---

# ByteArrayMessageDataConverter

Namespace: MassTransit.MessageData.Converters

```csharp
public class ByteArrayMessageDataConverter : IMessageDataConverter<Byte[]>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ByteArrayMessageDataConverter](../masstransit-messagedata-converters/bytearraymessagedataconverter)<br/>
Implements [IMessageDataConverter\<Byte[]\>](../masstransit-metadata/imessagedataconverter-1)

## Constructors

### **ByteArrayMessageDataConverter()**

```csharp
public ByteArrayMessageDataConverter()
```

## Methods

### **Convert(Stream, CancellationToken)**

```csharp
public Task<Byte[]> Convert(Stream stream, CancellationToken cancellationToken)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Byte[]\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
