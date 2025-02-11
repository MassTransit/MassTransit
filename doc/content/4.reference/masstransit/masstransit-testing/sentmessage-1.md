---

title: SentMessage<T>

---

# SentMessage\<T\>

Namespace: MassTransit.Testing

```csharp
public class SentMessage<T> : ISentMessage<T>, ISentMessage, IAsyncListElement
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SentMessage\<T\>](../masstransit-testing/sentmessage-1)<br/>
Implements [ISentMessage\<T\>](../masstransit-testing/isentmessage-1), [ISentMessage](../masstransit-testing/isentmessage), [IAsyncListElement](../masstransit-testing/iasynclistelement)

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

### **ShortTypeName**

```csharp
public string ShortTypeName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **SentMessage(SendContext\<T\>, Exception)**

```csharp
public SentMessage(SendContext<T> context, Exception exception)
```

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
