---

title: SendTransportContext

---

# SendTransportContext

Namespace: MassTransit.Transports

```csharp
public interface SendTransportContext : PipeContext, ISendObserverConnector
```

Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Properties

### **LogContext**

The LogContext used for sending transport messages, to ensure proper activity filtering

```csharp
public abstract ILogContext LogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **EntityName**

```csharp
public abstract string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityName**

```csharp
public abstract string ActivityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityDestination**

```csharp
public abstract string ActivityDestination { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivitySystem**

```csharp
public abstract string ActivitySystem { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SendObservers**

```csharp
public abstract SendObservable SendObservers { get; }
```

#### Property Value

[SendObservable](../../masstransit-abstractions/masstransit-observables/sendobservable)<br/>

### **Serialization**

```csharp
public abstract ISerialization Serialization { get; }
```

#### Property Value

[ISerialization](../../masstransit-abstractions/masstransit/iserialization)<br/>

## Methods

### **CreateSendContext\<T\>(T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Create the send context without the presence of a transport, but in a way that it can be used by the transport

```csharp
Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
