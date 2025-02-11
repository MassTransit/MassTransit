---

title: IFilterObserver<TContext>

---

# IFilterObserver\<TContext\>

Namespace: MassTransit

```csharp
public interface IFilterObserver<TContext>
```

#### Type Parameters

`TContext`<br/>

## Methods

### **PreSend(TContext)**

Called before a message is dispatched to any consumers

```csharp
Task PreSend(TContext context)
```

#### Parameters

`context` TContext<br/>
The consume context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend(TContext)**

Called after the message has been dispatched to all consumers - note that in the case of an exception
 this method is not called, and the DispatchFaulted method is called instead

```csharp
Task PostSend(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault(TContext, Exception)**

Called after the message has been dispatched to all consumers when one or more exceptions have occurred

```csharp
Task SendFault(TContext context, Exception exception)
```

#### Parameters

`context` TContext<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
