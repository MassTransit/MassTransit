---

title: IDispatchMetrics

---

# IDispatchMetrics

Namespace: MassTransit.Transports

```csharp
public interface IDispatchMetrics
```

## Properties

### **ActiveDispatchCount**

```csharp
public abstract int ActiveDispatchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **DispatchCount**

```csharp
public abstract long DispatchCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **MaxConcurrentDispatchCount**

```csharp
public abstract int MaxConcurrentDispatchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **GetMetrics()**

```csharp
DeliveryMetrics GetMetrics()
```

#### Returns

[DeliveryMetrics](../masstransit-transports/deliverymetrics)<br/>

## Events

### **ZeroActivity**

```csharp
public abstract event ZeroActiveDispatchHandler ZeroActivity;
```
