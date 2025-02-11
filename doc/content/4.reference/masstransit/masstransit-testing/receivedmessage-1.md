---

title: ReceivedMessage<T>

---

# ReceivedMessage\<T\>

Namespace: MassTransit.Testing

```csharp
public class ReceivedMessage<T> : IReceivedMessage<T>, IReceivedMessage, IAsyncListElement
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceivedMessage\<T\>](../masstransit-testing/receivedmessage-1)<br/>
Implements [IReceivedMessage\<T\>](../masstransit-testing/ireceivedmessage-1), [IReceivedMessage](../masstransit-testing/ireceivedmessage), [IAsyncListElement](../masstransit-testing/iasynclistelement)

## Properties

### **ElementId**

```csharp
public Nullable<Guid> ElementId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartTime**

```csharp
public DateTime StartTime { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **ElapsedTime**

```csharp
public TimeSpan ElapsedTime { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **ReceivedMessage(ConsumeContext\<T\>, Exception)**

```csharp
public ReceivedMessage(ConsumeContext<T> context, Exception exception)
```

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
