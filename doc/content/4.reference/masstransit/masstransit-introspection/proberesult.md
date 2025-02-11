---

title: ProbeResult

---

# ProbeResult

Namespace: MassTransit.Introspection

The result of a probe

```csharp
public interface ProbeResult
```

## Properties

### **ResultId**

Unique identifies this result

```csharp
public abstract Guid ResultId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ProbeId**

Identifies the initiator of the probe

```csharp
public abstract Guid ProbeId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **StartTimestamp**

When the probe was initiated through the system

```csharp
public abstract DateTime StartTimestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Duration**

How long the probe took to execute

```csharp
public abstract TimeSpan Duration { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Host**

The host from which the result was generated

```csharp
public abstract HostInfo Host { get; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **Results**

The results returned by the probe

```csharp
public abstract IDictionary<string, object> Results { get; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
