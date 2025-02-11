---

title: FaultEvent

---

# FaultEvent

Namespace: MassTransit.Events

```csharp
public class FaultEvent : Fault
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultEvent](../masstransit-events/faultevent)<br/>
Implements [Fault](../../masstransit-abstractions/masstransit/fault)

## Properties

### **FaultId**

```csharp
public Guid FaultId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **FaultedMessageId**

```csharp
public Nullable<Guid> FaultedMessageId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Exceptions**

```csharp
public ExceptionInfo[] Exceptions { get; set; }
```

#### Property Value

[ExceptionInfo[]](../../masstransit-abstractions/masstransit/exceptioninfo)<br/>

### **Host**

```csharp
public HostInfo Host { get; set; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **FaultMessageTypes**

```csharp
public String[] FaultMessageTypes { get; set; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **FaultEvent()**

```csharp
public FaultEvent()
```
