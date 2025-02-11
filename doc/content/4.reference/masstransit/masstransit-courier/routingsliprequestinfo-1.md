---

title: RoutingSlipRequestInfo<T>

---

# RoutingSlipRequestInfo\<T\>

Namespace: MassTransit.Courier

```csharp
public struct RoutingSlipRequestInfo<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [RoutingSlipRequestInfo\<T\>](../masstransit-courier/routingsliprequestinfo-1)

## Fields

### **RequestId**

```csharp
public Guid RequestId;
```

### **ResponseAddress**

```csharp
public Uri ResponseAddress;
```

### **FaultAddress**

```csharp
public Uri FaultAddress;
```

### **RequestAddress**

```csharp
public Uri RequestAddress;
```

### **RetryAttempt**

```csharp
public Nullable<int> RetryAttempt;
```

### **Request**

```csharp
public T Request;
```

## Constructors

### **RoutingSlipRequestInfo(IObjectDeserializer, IDictionary\<String, Object\>)**

```csharp
public RoutingSlipRequestInfo(IObjectDeserializer context, IDictionary<string, object> variables)
```

#### Parameters

`context` [IObjectDeserializer](../../masstransit-abstractions/masstransit/iobjectdeserializer)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
