---

title: IHealthCheckOptionsConfigurator

---

# IHealthCheckOptionsConfigurator

Namespace: MassTransit

```csharp
public interface IHealthCheckOptionsConfigurator
```

## Properties

### **Name**

Set the health check name, overrides the default bus type name

```csharp
public abstract string Name { set; }
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
public abstract Nullable<HealthStatus> FailureStatus { set; }
```

#### Property Value

[Nullable\<HealthStatus\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MinimalFailureStatus**

The minimal  that should be reported when the health check fails.
 If null then all statuses from  to  will be reported depending on app health.

```csharp
public abstract Nullable<HealthStatus> MinimalFailureStatus { set; }
```

#### Property Value

[Nullable\<HealthStatus\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Tags**

A list of tags that can be used to filter sets of health checks

```csharp
public abstract HashSet<string> Tags { get; }
```

#### Property Value

[HashSet\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1)<br/>
