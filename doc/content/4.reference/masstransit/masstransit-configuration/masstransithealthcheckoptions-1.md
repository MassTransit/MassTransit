---

title: MassTransitHealthCheckOptions<TBus>

---

# MassTransitHealthCheckOptions\<TBus\>

Namespace: MassTransit.Configuration

```csharp
public class MassTransitHealthCheckOptions<TBus> : IHealthCheckOptionsConfigurator, IHealthCheckOptions
```

#### Type Parameters

`TBus`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitHealthCheckOptions\<TBus\>](../masstransit-configuration/masstransithealthcheckoptions-1)<br/>
Implements [IHealthCheckOptionsConfigurator](../masstransit/ihealthcheckoptionsconfigurator), [IHealthCheckOptions](../masstransit-configuration/ihealthcheckoptions)

## Properties

### **Name**

The health check name. If null the type name of bus instance will be used

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **FailureStatus**

#### Caution

Use MinimalFailureStatus instead.

---

The  that should be reported when the health check fails.
 If null then the default status of  will be reported.

```csharp
public Nullable<HealthStatus> FailureStatus { get; set; }
```

#### Property Value

[Nullable\<HealthStatus\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MinimalFailureStatus**

The minimal  that should be reported when the health check fails.
 If null then all statuses from  to  will be reported depending on app health.

```csharp
public Nullable<HealthStatus> MinimalFailureStatus { get; set; }
```

#### Property Value

[Nullable\<HealthStatus\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Tags**

A list of tags that can be used to filter sets of health checks. If empty, the default tags
 will be used.

```csharp
public HashSet<string> Tags { get; }
```

#### Property Value

[HashSet\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1)<br/>

## Constructors

### **MassTransitHealthCheckOptions()**

```csharp
public MassTransitHealthCheckOptions()
```
