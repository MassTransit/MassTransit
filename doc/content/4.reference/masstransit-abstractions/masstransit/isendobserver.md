---

title: ISendObserver

---

# ISendObserver

Namespace: MassTransit

Observes messages as they are sent to transports. These should not be used to intercept or
 filter messages, in that case a filter should be created and registered on the transport.

```csharp
public interface ISendObserver
```

## Methods

### **PreSend\<T\>(SendContext\<T\>)**

Called before the message is sent to the transport

```csharp
Task PreSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>
The message send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend\<T\>(SendContext\<T\>)**

Called after the message is sent to the transport (and confirmed by the transport if supported)

```csharp
Task PostSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>
The message send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault\<T\>(SendContext\<T\>, Exception)**

Called when the message fails to send to the transport, including the exception that was thrown

```csharp
Task SendFault<T>(SendContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>
The message send context

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception from the transport

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
