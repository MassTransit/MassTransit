---

title: ScheduledMessageHandle<T>

---

# ScheduledMessageHandle\<T\>

Namespace: MassTransit.Scheduling

```csharp
public class ScheduledMessageHandle<T> : ScheduledMessage<T>, ScheduledMessage
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduledMessageHandle\<T\>](../masstransit-scheduling/scheduledmessagehandle-1)<br/>
Implements [ScheduledMessage\<T\>](../../masstransit-abstractions/masstransit/scheduledmessage-1), [ScheduledMessage](../../masstransit-abstractions/masstransit/scheduledmessage)

## Properties

### **TokenId**

```csharp
public Guid TokenId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ScheduledTime**

```csharp
public DateTime ScheduledTime { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Destination**

```csharp
public Uri Destination { get; }
```

#### Property Value

Uri<br/>

### **Payload**

```csharp
public T Payload { get; }
```

#### Property Value

T<br/>

## Constructors

### **ScheduledMessageHandle(Guid, DateTime, Uri, T)**

```csharp
public ScheduledMessageHandle(Guid tokenId, DateTime scheduledTime, Uri destination, T payload)
```

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`destination` Uri<br/>

`payload` T<br/>
