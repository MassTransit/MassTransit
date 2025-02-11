---

title: StatsDConfiguration

---

# StatsDConfiguration

Namespace: MassTransit.Monitoring.Performance.StatsD

```csharp
public class StatsDConfiguration
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StatsDConfiguration](../masstransit-monitoring-performance-statsd/statsdconfiguration)

## Properties

### **Hostname**

```csharp
public string Hostname { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Port**

```csharp
public int Port { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **StatsDConfiguration(String, Int32)**

```csharp
public StatsDConfiguration(string hostname, int port)
```

#### Parameters

`hostname` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`port` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Defaults()**

```csharp
public static StatsDConfiguration Defaults()
```

#### Returns

[StatsDConfiguration](../masstransit-monitoring-performance-statsd/statsdconfiguration)<br/>
