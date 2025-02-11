---

title: StringMessageDataConverter

---

# StringMessageDataConverter

Namespace: MassTransit.MessageData.Converters

```csharp
public class StringMessageDataConverter : IMessageDataConverter<String>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StringMessageDataConverter](../masstransit-messagedata-converters/stringmessagedataconverter)<br/>
Implements [IMessageDataConverter\<String\>](../masstransit-metadata/imessagedataconverter-1)

## Constructors

### **StringMessageDataConverter()**

```csharp
public StringMessageDataConverter()
```

## Methods

### **Convert(Stream, CancellationToken)**

```csharp
public Task<string> Convert(Stream stream, CancellationToken cancellationToken)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
