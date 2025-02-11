---

title: CompensateLog

---

# CompensateLog

Namespace: MassTransit.Courier.Contracts

```csharp
public interface CompensateLog
```

## Properties

### **ExecutionId**

The tracking number for completion of the activity

```csharp
public abstract Guid ExecutionId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Address**

The compensation address where the routing slip should be sent for compensation

```csharp
public abstract Uri Address { get; }
```

#### Property Value

Uri<br/>

### **Data**

The results of the activity saved for compensation

```csharp
public abstract IDictionary<string, object> Data { get; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
