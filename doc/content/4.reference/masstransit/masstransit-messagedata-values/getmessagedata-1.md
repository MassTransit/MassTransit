---

title: GetMessageData<T>

---

# GetMessageData\<T\>

Namespace: MassTransit.MessageData.Values

Gets the message data when accessed via Value, using the specified repository and converter.

```csharp
public class GetMessageData<T> : MessageData<T>, IMessageData
```

#### Type Parameters

`T`<br/>
The message data property type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GetMessageData\<T\>](../masstransit-messagedata-values/getmessagedata-1)<br/>
Implements [MessageData\<T\>](../../masstransit-abstractions/masstransit/messagedata-1), [IMessageData](../../masstransit-abstractions/masstransit/imessagedata)

## Properties

### **Address**

```csharp
public Uri Address { get; }
```

#### Property Value

Uri<br/>

### **HasValue**

```csharp
public bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Value**

```csharp
public Task<T> Value { get; }
```

#### Property Value

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **GetMessageData(Uri, IMessageDataRepository, IMessageDataConverter\<T\>, CancellationToken)**

```csharp
public GetMessageData(Uri address, IMessageDataRepository repository, IMessageDataConverter<T> converter, CancellationToken cancellationToken)
```

#### Parameters

`address` Uri<br/>

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`converter` [IMessageDataConverter\<T\>](../masstransit-metadata/imessagedataconverter-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
