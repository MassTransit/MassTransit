---

title: DeserializerConsumeContext

---

# DeserializerConsumeContext

Namespace: MassTransit.Context

```csharp
public abstract class DeserializerConsumeContext : BaseConsumeContext, IPublishEndpoint, IPublishObserverConnector, ConsumeContext, PipeContext, MessageContext, ISendEndpointProvider, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpoint](../../masstransit-abstractions/masstransit-transports/publishendpoint) → [BaseConsumeContext](../masstransit-context/baseconsumecontext) → [DeserializerConsumeContext](../masstransit-context/deserializerconsumecontext)<br/>
Implements [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Properties

### **ConsumeCompleted**

```csharp
public Task ConsumeCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **ReceiveContext**

```csharp
public ReceiveContext ReceiveContext { get; protected set; }
```

#### Property Value

[ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

### **SerializerContext**

```csharp
public SerializerContext SerializerContext { get; }
```

#### Property Value

[SerializerContext](../../masstransit-abstractions/masstransit/serializercontext)<br/>

### **MessageId**

```csharp
public abstract Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public abstract Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public abstract Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public abstract Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public abstract Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

```csharp
public abstract Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public abstract Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public abstract Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public abstract Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public abstract Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

```csharp
public abstract Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public abstract Headers Headers { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **Host**

```csharp
public abstract HostInfo Host { get; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **SupportedMessageTypes**

```csharp
public abstract IEnumerable<string> SupportedMessageTypes { get; }
```

#### Property Value

[IEnumerable\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **HasPayloadType(Type)**

Returns true if the payload type is included with or supported by the context type

```csharp
public bool HasPayloadType(Type payloadType)
```

#### Parameters

`payloadType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPayload\<T\>(T)**

Attempts to get the specified payload type

```csharp
public bool TryGetPayload<T>(out T payload)
```

#### Type Parameters

`T`<br/>

#### Parameters

`payload` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetOrAddPayload\<T\>(PayloadFactory\<T\>)**

Get or add a payload to the context, using the provided payload factory.

```csharp
public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
```

#### Type Parameters

`T`<br/>
The payload type

#### Parameters

`payloadFactory` [PayloadFactory\<T\>](../../masstransit-abstractions/masstransit/payloadfactory-1)<br/>
The payload factory, which is only invoked if the payload is not present.

#### Returns

T<br/>

### **AddOrUpdatePayload\<T\>(PayloadFactory\<T\>, UpdatePayloadFactory\<T\>)**

Either adds a new payload, or updates an existing payload

```csharp
public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
```

#### Type Parameters

`T`<br/>
The payload type

#### Parameters

`addFactory` [PayloadFactory\<T\>](../../masstransit-abstractions/masstransit/payloadfactory-1)<br/>
The payload factory called if the payload is not present

`updateFactory` [UpdatePayloadFactory\<T\>](../../masstransit-abstractions/masstransit/updatepayloadfactory-1)<br/>
The payload factory called if the payload already exists

#### Returns

T<br/>

### **AddConsumeTask(Task)**

```csharp
public void AddConsumeTask(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
