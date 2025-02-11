---

title: ScheduleSendPipe<TMessage>

---

# ScheduleSendPipe\<TMessage\>

Namespace: MassTransit.Scheduling

For transport-based schedulers, used to invoke the  pipe and
 manage the ScheduledMessageId, as well as set the transport delay property

```csharp
public class ScheduleSendPipe<TMessage> : SendContextPipeAdapter<TMessage>, IPipe<SendContext<TMessage>>, IProbeSite, ISendPipe, ISendContextPipe
```

#### Type Parameters

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendContextPipeAdapter\<TMessage\>](../masstransit-transports/sendcontextpipeadapter-1) → [ScheduleSendPipe\<TMessage\>](../masstransit-scheduling/schedulesendpipe-1)<br/>
Implements [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ISendPipe](../../masstransit-abstractions/masstransit-transports/isendpipe), [ISendContextPipe](../../masstransit-abstractions/masstransit-transports/isendcontextpipe)

## Properties

### **ScheduledMessageId**

```csharp
public Nullable<Guid> ScheduledMessageId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **ScheduleSendPipe(IPipe\<SendContext\<TMessage\>\>, DateTime)**

```csharp
public ScheduleSendPipe(IPipe<SendContext<TMessage>> pipe, DateTime scheduledTime)
```

#### Parameters

`pipe` [IPipe\<SendContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

## Methods

### **Send(SendContext\<TMessage\>)**

```csharp
protected void Send(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

### **Send\<T\>(SendContext\<T\>)**

```csharp
protected void Send<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>
