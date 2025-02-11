---

title: BusHealthResult

---

# BusHealthResult

Namespace: MassTransit

```csharp
public class BusHealthResult
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusHealthResult](../masstransit/bushealthresult)

## Fields

### **Endpoints**

```csharp
public IReadOnlyDictionary<string, EndpointHealthResult> Endpoints;
```

### **Exception**

```csharp
public Exception Exception;
```

### **Status**

```csharp
public BusHealthStatus Status;
```

## Properties

### **Description**

```csharp
public string Description { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Healthy(String, IReadOnlyDictionary\<String, EndpointHealthResult\>)**

```csharp
public static BusHealthResult Healthy(string description, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
```

#### Parameters

`description` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`endpoints` [IReadOnlyDictionary\<String, EndpointHealthResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

#### Returns

[BusHealthResult](../masstransit/bushealthresult)<br/>

### **Degraded(String, Exception, IReadOnlyDictionary\<String, EndpointHealthResult\>)**

```csharp
public static BusHealthResult Degraded(string description, Exception exception, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
```

#### Parameters

`description` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`endpoints` [IReadOnlyDictionary\<String, EndpointHealthResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

#### Returns

[BusHealthResult](../masstransit/bushealthresult)<br/>

### **Unhealthy(String, Exception, IReadOnlyDictionary\<String, EndpointHealthResult\>)**

```csharp
public static BusHealthResult Unhealthy(string description, Exception exception, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
```

#### Parameters

`description` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`endpoints` [IReadOnlyDictionary\<String, EndpointHealthResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

#### Returns

[BusHealthResult](../masstransit/bushealthresult)<br/>
