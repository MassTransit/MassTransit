---

title: EndpointHealthResult

---

# EndpointHealthResult

Namespace: MassTransit

```csharp
public struct EndpointHealthResult
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [EndpointHealthResult](../masstransit/endpointhealthresult)

## Fields

### **InputAddress**

```csharp
public Uri InputAddress;
```

### **Status**

```csharp
public BusHealthStatus Status;
```

### **Description**

```csharp
public string Description;
```

### **Exception**

```csharp
public Exception Exception;
```

### **ReceiveEndpoint**

```csharp
public IReceiveEndpoint ReceiveEndpoint;
```

## Methods

### **Healthy(IReceiveEndpoint, String)**

```csharp
public static EndpointHealthResult Healthy(IReceiveEndpoint receiveEndpoint, string description)
```

#### Parameters

`receiveEndpoint` [IReceiveEndpoint](../masstransit/ireceiveendpoint)<br/>

`description` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[EndpointHealthResult](../masstransit/endpointhealthresult)<br/>

### **Degraded(IReceiveEndpoint, String)**

```csharp
public static EndpointHealthResult Degraded(IReceiveEndpoint receiveEndpoint, string description)
```

#### Parameters

`receiveEndpoint` [IReceiveEndpoint](../masstransit/ireceiveendpoint)<br/>

`description` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[EndpointHealthResult](../masstransit/endpointhealthresult)<br/>

### **Unhealthy(IReceiveEndpoint, String, Exception)**

```csharp
public static EndpointHealthResult Unhealthy(IReceiveEndpoint receiveEndpoint, string description, Exception exception)
```

#### Parameters

`receiveEndpoint` [IReceiveEndpoint](../masstransit/ireceiveendpoint)<br/>

`description` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[EndpointHealthResult](../masstransit/endpointhealthresult)<br/>
