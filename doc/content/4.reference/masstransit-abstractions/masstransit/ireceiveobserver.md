---

title: IReceiveObserver

---

# IReceiveObserver

Namespace: MassTransit

An observer that can monitor a receive endpoint to track message consumption at the
 endpoint level.

```csharp
public interface IReceiveObserver
```

## Methods

### **PreReceive(ReceiveContext)**

Called when a message has been delivered by the transport is about to be received by the endpoint

```csharp
Task PreReceive(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>
The receive context of the message

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostReceive(ReceiveContext)**

Called when the message has been received and acknowledged on the transport

```csharp
Task PostReceive(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>
The receive context of the message

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume\<T\>(ConsumeContext\<T\>, TimeSpan, String)**

Called when a message has been consumed by a consumer

```csharp
Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>
The message consume context

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The consumer duration

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The consumer type

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault\<T\>(ConsumeContext\<T\>, TimeSpan, String, Exception)**

Called when a message being consumed produced a fault

```csharp
Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>
The message consume context

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The consumer duration

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The consumer type

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception from the consumer

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ReceiveFault(ReceiveContext, Exception)**

Called when the transport receive faults

```csharp
Task ReceiveFault(ReceiveContext context, Exception exception)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>
The receive context of the message

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception that was thrown

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
