---

title: ISendTransport

---

# ISendTransport

Namespace: MassTransit.Transports

```csharp
public interface ISendTransport : ISendObserverConnector
```

Implements [ISendObserverConnector](../masstransit/isendobserverconnector)

## Methods

### **CreateSendContext\<T\>(T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Send\<T\>(T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Send a message to the transport. The transport creates the OldSendContext, and calls back to
 allow the context to be modified to customize the message delivery.
 The transport specifies the defaults for the message as configured, and then allows the
 caller to modify the send context to include the required settings (durable, mandatory, etc.).

```csharp
Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>
The pipe invoked when sending a message, to do extra stuff

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
Cancel the send operation (if possible)

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker
