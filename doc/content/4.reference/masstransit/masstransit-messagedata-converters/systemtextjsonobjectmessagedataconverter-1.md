---

title: SystemTextJsonObjectMessageDataConverter<T>

---

# SystemTextJsonObjectMessageDataConverter\<T\>

Namespace: MassTransit.MessageData.Converters

```csharp
public class SystemTextJsonObjectMessageDataConverter<T> : IMessageDataConverter<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTextJsonObjectMessageDataConverter\<T\>](../masstransit-messagedata-converters/systemtextjsonobjectmessagedataconverter-1)<br/>
Implements [IMessageDataConverter\<T\>](../masstransit-metadata/imessagedataconverter-1)

## Constructors

### **SystemTextJsonObjectMessageDataConverter(JsonSerializerOptions)**

```csharp
public SystemTextJsonObjectMessageDataConverter(JsonSerializerOptions options)
```

#### Parameters

`options` JsonSerializerOptions<br/>

## Methods

### **Convert(Stream, CancellationToken)**

```csharp
public Task<T> Convert(Stream stream, CancellationToken cancellationToken)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
