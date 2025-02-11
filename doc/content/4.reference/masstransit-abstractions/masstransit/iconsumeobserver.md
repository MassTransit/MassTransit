---

title: IConsumeObserver

---

# IConsumeObserver

Namespace: MassTransit

Intercepts the ConsumeContext

```csharp
public interface IConsumeObserver
```

## Methods

### **PreConsume\<T\>(ConsumeContext\<T\>)**

Called before a message is dispatched to any consumers

```csharp
Task PreConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>
The consume context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume\<T\>(ConsumeContext\<T\>)**

Called after the message has been dispatched to all consumers - note that in the case of an exception
 this method is not called, and the DispatchFaulted method is called instead

```csharp
Task PostConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault\<T\>(ConsumeContext\<T\>, Exception)**

Called after the message has been dispatched to all consumers when one or more exceptions have occurred

```csharp
Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
