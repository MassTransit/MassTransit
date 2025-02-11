---

title: FaultEvent<T>

---

# FaultEvent\<T\>

Namespace: MassTransit.Events

```csharp
public class FaultEvent<T> : Fault<T>, Fault
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultEvent\<T\>](../masstransit-events/faultevent-1)<br/>
Implements [Fault\<T\>](../../masstransit-abstractions/masstransit/fault-1), [Fault](../../masstransit-abstractions/masstransit/fault)

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

### **Message**

```csharp
public T Message { get; set; }
```

#### Property Value

T<br/>

## Constructors

### **FaultEvent()**

```csharp
public FaultEvent()
```

### **FaultEvent(T, Nullable\<Guid\>, HostInfo, Exception, String[])**

```csharp
public FaultEvent(T message, Nullable<Guid> faultedMessageId, HostInfo host, Exception exception, String[] faultMessageTypes)
```

#### Parameters

`message` T<br/>

`faultedMessageId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`host` [HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`faultMessageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **FaultEvent(T, Nullable\<Guid\>, HostInfo, IEnumerable\<ExceptionInfo\>, String[])**

```csharp
public FaultEvent(T message, Nullable<Guid> faultedMessageId, HostInfo host, IEnumerable<ExceptionInfo> exceptions, String[] faultMessageTypes)
```

#### Parameters

`message` T<br/>

`faultedMessageId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`host` [HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

`exceptions` [IEnumerable\<ExceptionInfo\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`faultMessageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
