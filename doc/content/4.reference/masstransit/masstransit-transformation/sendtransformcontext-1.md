---

title: SendTransformContext<TMessage>

---

# SendTransformContext\<TMessage\>

Namespace: MassTransit.Transformation

Sits in front of the consume context and allows the inbound message to be
 transformed.

```csharp
public class SendTransformContext<TMessage> : ProxyPipeContext, TransformContext<TMessage>, TransformContext, PipeContext, MessageContext
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProxyPipeContext](../../masstransit-abstractions/masstransit-middleware/proxypipecontext) → [SendTransformContext\<TMessage\>](../masstransit-transformation/sendtransformcontext-1)<br/>
Implements [TransformContext\<TMessage\>](../../masstransit-abstractions/masstransit/transformcontext-1), [TransformContext](../../masstransit-abstractions/masstransit/transformcontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)

## Properties

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public Headers Headers { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **Host**

```csharp
public HostInfo Host { get; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **HasInput**

```csharp
public bool HasInput { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Input**

```csharp
public TMessage Input { get; }
```

#### Property Value

TMessage<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **SendTransformContext(SendContext\<TMessage\>)**

```csharp
public SendTransformContext(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>
