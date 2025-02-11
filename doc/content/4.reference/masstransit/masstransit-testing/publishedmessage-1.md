---

title: PublishedMessage<T>

---

# PublishedMessage\<T\>

Namespace: MassTransit.Testing

```csharp
public class PublishedMessage<T> : IPublishedMessage<T>, IPublishedMessage, IAsyncListElement
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishedMessage\<T\>](../masstransit-testing/publishedmessage-1)<br/>
Implements [IPublishedMessage\<T\>](../masstransit-testing/ipublishedmessage-1), [IPublishedMessage](../masstransit-testing/ipublishedmessage), [IAsyncListElement](../masstransit-testing/iasynclistelement)

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

### **Exception**

```csharp
public Exception Exception { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **ShortTypeName**

```csharp
public string ShortTypeName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **PublishedMessage(PublishContext\<T\>, Exception)**

```csharp
public PublishedMessage(PublishContext<T> context, Exception exception)
```

#### Parameters

`context` [PublishContext\<T\>](../../masstransit-abstractions/masstransit/publishcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
