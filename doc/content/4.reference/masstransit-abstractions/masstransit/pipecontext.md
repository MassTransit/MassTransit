---

title: PipeContext

---

# PipeContext

Namespace: MassTransit

The base context for all pipe types, includes the payload side-banding of data
 with the payload, as well as the cancellationToken to avoid passing it everywhere

```csharp
public interface PipeContext
```

## Properties

### **CancellationToken**

Used to cancel the execution of the context

```csharp
public abstract CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **HasPayloadType(Type)**

Checks if a payload is present in the context

```csharp
bool HasPayloadType(Type payloadType)
```

#### Parameters

`payloadType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPayload\<T\>(T)**

Retrieves a payload from the pipe context

```csharp
bool TryGetPayload<T>(out T payload)
```

#### Type Parameters

`T`<br/>
The payload type

#### Parameters

`payload` T<br/>
The payload

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetOrAddPayload\<T\>(PayloadFactory\<T\>)**

Returns an existing payload or creates the payload using the factory method provided

```csharp
T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
```

#### Type Parameters

`T`<br/>
The payload type

#### Parameters

`payloadFactory` [PayloadFactory\<T\>](../masstransit/payloadfactory-1)<br/>
The payload factory to use if the payload is not already present

#### Returns

T<br/>
The payload

### **AddOrUpdatePayload\<T\>(PayloadFactory\<T\>, UpdatePayloadFactory\<T\>)**

Either adds a new payload, or updates an existing payload

```csharp
T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
```

#### Type Parameters

`T`<br/>
The payload type

#### Parameters

`addFactory` [PayloadFactory\<T\>](../masstransit/payloadfactory-1)<br/>
The payload factory called if the payload is not present

`updateFactory` [UpdatePayloadFactory\<T\>](../masstransit/updatepayloadfactory-1)<br/>
The payload factory called if the payload already exists

#### Returns

T<br/>
