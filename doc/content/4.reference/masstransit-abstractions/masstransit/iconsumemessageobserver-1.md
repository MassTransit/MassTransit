---

title: IConsumeMessageObserver<T>

---

# IConsumeMessageObserver\<T\>

Namespace: MassTransit

Intercepts the ConsumeContext

```csharp
public interface IConsumeMessageObserver<T>
```

#### Type Parameters

`T`<br/>
The message type

## Methods

### **PreConsume(ConsumeContext\<T\>)**

Called before a message is dispatched to any consumers

```csharp
Task PreConsume(ConsumeContext<T> context)
```

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>
The consume context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume(ConsumeContext\<T\>)**

Called after the message has been dispatched to all consumers - note that in the case of an exception
 this method is not called, and the DispatchFaulted method is called instead

```csharp
Task PostConsume(ConsumeContext<T> context)
```

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault(ConsumeContext\<T\>, Exception)**

Called after the message has been dispatched to all consumers when one or more exceptions have occurred

```csharp
Task ConsumeFault(ConsumeContext<T> context, Exception exception)
```

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
