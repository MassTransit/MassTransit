---

title: IPayloadCache

---

# IPayloadCache

Namespace: MassTransit.Payloads

The context properties

```csharp
public interface IPayloadCache
```

## Methods

### **HasPayloadType(Type)**

Checks if the property exists in the cache

```csharp
bool HasPayloadType(Type payloadType)
```

#### Parameters

`payloadType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The property type

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the property exists in the cache, otherwise false

### **TryGetPayload\<TPayload\>(TPayload)**

Returns the value of the property if it exists in the cache

```csharp
bool TryGetPayload<TPayload>(out TPayload payload)
```

#### Type Parameters

`TPayload`<br/>
The property type

#### Parameters

`payload` TPayload<br/>
The property value

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the value was returned, otherwise false

### **GetOrAddPayload\<T\>(PayloadFactory\<T\>)**

Return an existing or create a new property

```csharp
T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`payloadFactory` [PayloadFactory\<T\>](../masstransit/payloadfactory-1)<br/>

#### Returns

T<br/>

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
