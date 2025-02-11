---

title: ListPayloadCache

---

# ListPayloadCache

Namespace: MassTransit.Payloads

```csharp
public class ListPayloadCache : IPayloadCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ListPayloadCache](../masstransit-payloads/listpayloadcache)<br/>
Implements [IPayloadCache](../masstransit-payloads/ipayloadcache)

## Constructors

### **ListPayloadCache()**

```csharp
public ListPayloadCache()
```

### **ListPayloadCache(Object[])**

```csharp
public ListPayloadCache(Object[] payloads)
```

#### Parameters

`payloads` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Methods

### **HasPayloadType(Type)**

```csharp
public bool HasPayloadType(Type payloadType)
```

#### Parameters

`payloadType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPayload\<TPayload\>(TPayload)**

```csharp
public bool TryGetPayload<TPayload>(out TPayload payload)
```

#### Type Parameters

`TPayload`<br/>

#### Parameters

`payload` TPayload<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetOrAddPayload\<T\>(PayloadFactory\<T\>)**

```csharp
public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`payloadFactory` [PayloadFactory\<T\>](../masstransit/payloadfactory-1)<br/>

#### Returns

T<br/>

### **AddOrUpdatePayload\<T\>(PayloadFactory\<T\>, UpdatePayloadFactory\<T\>)**

```csharp
public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`addFactory` [PayloadFactory\<T\>](../masstransit/payloadfactory-1)<br/>

`updateFactory` [UpdatePayloadFactory\<T\>](../masstransit/updatepayloadfactory-1)<br/>

#### Returns

T<br/>
