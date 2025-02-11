---

title: BindContextProxy<TLeft, TRight>

---

# BindContextProxy\<TLeft, TRight\>

Namespace: MassTransit.Context

The BindContext

```csharp
public class BindContextProxy<TLeft, TRight> : BindContext<TLeft, TRight>, PipeContext
```

#### Type Parameters

`TLeft`<br/>

`TRight`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BindContextProxy\<TLeft, TRight\>](../masstransit-context/bindcontextproxy-2)<br/>
Implements [BindContext\<TLeft, TRight\>](../../masstransit-abstractions/masstransit/bindcontext-2), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Properties

### **Left**

```csharp
public TLeft Left { get; }
```

#### Property Value

TLeft<br/>

### **Right**

```csharp
public TRight Right { get; }
```

#### Property Value

TRight<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **BindContextProxy(TLeft, TRight)**

```csharp
public BindContextProxy(TLeft left, TRight source)
```

#### Parameters

`left` TLeft<br/>

`source` TRight<br/>

## Methods

### **HasPayloadType(Type)**

```csharp
public bool HasPayloadType(Type payloadType)
```

#### Parameters

`payloadType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPayload\<T\>(T)**

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

```csharp
public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`payloadFactory` [PayloadFactory\<T\>](../../masstransit-abstractions/masstransit/payloadfactory-1)<br/>

#### Returns

T<br/>

### **AddOrUpdatePayload\<T\>(PayloadFactory\<T\>, UpdatePayloadFactory\<T\>)**

```csharp
public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`addFactory` [PayloadFactory\<T\>](../../masstransit-abstractions/masstransit/payloadfactory-1)<br/>

`updateFactory` [UpdatePayloadFactory\<T\>](../../masstransit-abstractions/masstransit/updatepayloadfactory-1)<br/>

#### Returns

T<br/>
