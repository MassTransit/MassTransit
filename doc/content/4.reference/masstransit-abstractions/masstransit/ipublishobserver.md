---

title: IPublishObserver

---

# IPublishObserver

Namespace: MassTransit

Observes messages as they are published via a publish endpoint. These should not be used to intercept or
 filter messages, in that case a filter should be created and registered on the transport.

```csharp
public interface IPublishObserver
```

## Methods

### **PrePublish\<T\>(PublishContext\<T\>)**

Called before the message is sent to the transport

```csharp
Task PrePublish<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [PublishContext\<T\>](../masstransit/publishcontext-1)<br/>
The message send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostPublish\<T\>(PublishContext\<T\>)**

Called after the message is sent to the transport (and confirmed by the transport if supported)

```csharp
Task PostPublish<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [PublishContext\<T\>](../masstransit/publishcontext-1)<br/>
The message send context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishFault\<T\>(PublishContext\<T\>, Exception)**

Called when the message fails to send to the transport, including the exception that was thrown

```csharp
Task PublishFault<T>(PublishContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [PublishContext\<T\>](../masstransit/publishcontext-1)<br/>
The message send context

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception from the transport

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
