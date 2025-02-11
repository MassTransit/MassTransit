---

title: Fault

---

# Fault

Namespace: MassTransit

Published (or sent, if part of a request/response conversation) when a fault occurs during message
 processing

```csharp
public interface Fault
```

## Properties

### **FaultId**

Identifies the fault that was generated

```csharp
public abstract Guid FaultId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **FaultedMessageId**

The messageId that faulted

```csharp
public abstract Nullable<Guid> FaultedMessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Timestamp**

When the fault was produced

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Exceptions**

The exception information that occurred

```csharp
public abstract ExceptionInfo[] Exceptions { get; }
```

#### Property Value

[ExceptionInfo[]](../masstransit/exceptioninfo)<br/>

### **Host**

The host information was the fault occurred

```csharp
public abstract HostInfo Host { get; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

### **FaultMessageTypes**

The faulted message supported types, from the original message envelope

```csharp
public abstract String[] FaultMessageTypes { get; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
