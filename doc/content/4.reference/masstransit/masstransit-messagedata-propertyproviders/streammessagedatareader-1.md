---

title: StreamMessageDataReader<T>

---

# StreamMessageDataReader\<T\>

Namespace: MassTransit.MessageData.PropertyProviders

```csharp
public class StreamMessageDataReader<T> : IMessageDataReader<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StreamMessageDataReader\<T\>](../masstransit-messagedata-propertyproviders/streammessagedatareader-1)<br/>
Implements [IMessageDataReader\<T\>](../masstransit-messagedata-propertyproviders/imessagedatareader-1)

## Constructors

### **StreamMessageDataReader()**

```csharp
public StreamMessageDataReader()
```

## Methods

### **GetMessageData(IMessageDataRepository, Uri, CancellationToken)**

```csharp
public MessageData<T> GetMessageData(IMessageDataRepository repository, Uri address, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`address` Uri<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[MessageData\<T\>](../../masstransit-abstractions/masstransit/messagedata-1)<br/>
