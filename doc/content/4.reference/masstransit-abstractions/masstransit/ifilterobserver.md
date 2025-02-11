---

title: IFilterObserver

---

# IFilterObserver

Namespace: MassTransit

```csharp
public interface IFilterObserver
```

## Methods

### **PreSend\<T\>(T)**

Called before a message is dispatched to any consumers

```csharp
Task PreSend<T>(T context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` T<br/>
The consume context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend\<T\>(T)**

Called after the message has been dispatched to all consumers - note that in the case of an exception
 this method is not called, and the DispatchFaulted method is called instead

```csharp
Task PostSend<T>(T context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault\<T\>(T, Exception)**

Called after the message has been dispatched to all consumers when one or more exceptions have occurred

```csharp
Task SendFault<T>(T context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` T<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
