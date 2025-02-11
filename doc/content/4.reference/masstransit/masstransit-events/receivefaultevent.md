---

title: ReceiveFaultEvent

---

# ReceiveFaultEvent

Namespace: MassTransit.Events

```csharp
public class ReceiveFaultEvent : ReceiveFault, Fault
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveFaultEvent](../masstransit-events/receivefaultevent)<br/>
Implements [ReceiveFault](../../masstransit-abstractions/masstransit/receivefault), [Fault](../../masstransit-abstractions/masstransit/fault)

## Properties

### **FaultId**

```csharp
public Guid FaultId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **FaultedMessageId**

```csharp
public Nullable<Guid> FaultedMessageId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

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

### **ContentType**

```csharp
public string ContentType { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **ReceiveFaultEvent()**

```csharp
public ReceiveFaultEvent()
```

### **ReceiveFaultEvent(HostInfo, Exception, String, Nullable\<Guid\>, String[])**

```csharp
public ReceiveFaultEvent(HostInfo host, Exception exception, string contentType, Nullable<Guid> faultedMessageId, String[] faultMessageTypes)
```

#### Parameters

`host` [HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`contentType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`faultedMessageId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`faultMessageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
