---

title: OutboxMessageContext

---

# OutboxMessageContext

Namespace: MassTransit.Middleware

```csharp
public interface OutboxMessageContext : MessageContext
```

Implements [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)

## Properties

### **SequenceNumber**

```csharp
public abstract long SequenceNumber { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **MessageId**

```csharp
public abstract Guid MessageId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ContentType**

```csharp
public abstract string ContentType { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MessageType**

```csharp
public abstract string MessageType { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Body**

```csharp
public abstract string Body { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Properties**

```csharp
public abstract IReadOnlyDictionary<string, object> Properties { get; }
```

#### Property Value

[IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>
