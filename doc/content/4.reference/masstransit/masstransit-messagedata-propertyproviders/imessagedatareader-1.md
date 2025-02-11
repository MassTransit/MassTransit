---

title: IMessageDataReader<T>

---

# IMessageDataReader\<T\>

Namespace: MassTransit.MessageData.PropertyProviders

```csharp
public interface IMessageDataReader<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **GetMessageData(IMessageDataRepository, Uri, CancellationToken)**

```csharp
MessageData<T> GetMessageData(IMessageDataRepository repository, Uri address, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`address` Uri<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[MessageData\<T\>](../../masstransit-abstractions/masstransit/messagedata-1)<br/>
