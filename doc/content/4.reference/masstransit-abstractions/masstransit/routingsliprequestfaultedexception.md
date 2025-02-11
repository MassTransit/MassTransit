---

title: RoutingSlipRequestFaultedException

---

# RoutingSlipRequestFaultedException

Namespace: MassTransit

```csharp
public class RoutingSlipRequestFaultedException : RoutingSlipException, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception) → [MassTransitException](../masstransit/masstransitexception) → [CourierException](../masstransit/courierexception) → [RoutingSlipException](../masstransit/routingslipexception) → [RoutingSlipRequestFaultedException](../masstransit/routingsliprequestfaultedexception)<br/>
Implements [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)

## Properties

### **Faulted**

```csharp
public RoutingSlipFaulted Faulted { get; }
```

#### Property Value

[RoutingSlipFaulted](../masstransit-courier-contracts/routingslipfaulted)<br/>

### **TargetSite**

```csharp
public MethodBase TargetSite { get; }
```

#### Property Value

[MethodBase](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbase)<br/>

### **Message**

```csharp
public string Message { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Data**

```csharp
public IDictionary Data { get; }
```

#### Property Value

[IDictionary](https://learn.microsoft.com/en-us/dotnet/api/system.collections.idictionary)<br/>

### **InnerException**

```csharp
public Exception InnerException { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **HelpLink**

```csharp
public string HelpLink { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Source**

```csharp
public string Source { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **HResult**

```csharp
public int HResult { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **StackTrace**

```csharp
public string StackTrace { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **RoutingSlipRequestFaultedException(RoutingSlipFaulted)**

```csharp
public RoutingSlipRequestFaultedException(RoutingSlipFaulted faulted)
```

#### Parameters

`faulted` [RoutingSlipFaulted](../masstransit-courier-contracts/routingslipfaulted)<br/>
