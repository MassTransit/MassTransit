---

title: IActivityObserver

---

# IActivityObserver

Namespace: MassTransit

```csharp
public interface IActivityObserver
```

## Methods

### **PreExecute\<TActivity, TArguments\>(ExecuteActivityContext\<TActivity, TArguments\>)**

Called before a message is dispatched to any consumers

```csharp
Task PreExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../masstransit/executeactivitycontext-2)<br/>
The consume context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostExecute\<TActivity, TArguments\>(ExecuteActivityContext\<TActivity, TArguments\>)**

Called after the message has been dispatched to all consumers - note that in the case of an exception
 this method is not called, and the DispatchFaulted method is called instead

```csharp
Task PostExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../masstransit/executeactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ExecuteFault\<TActivity, TArguments\>(ExecuteActivityContext\<TActivity, TArguments\>, Exception)**

Called after the message has been dispatched to all consumers when one or more exceptions have occurred

```csharp
Task ExecuteFault<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../masstransit/executeactivitycontext-2)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreCompensate\<TActivity, TLog\>(CompensateActivityContext\<TActivity, TLog\>)**

Called before a message is dispatched to any consumers

```csharp
Task PreCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../masstransit/compensateactivitycontext-2)<br/>
The consume context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostCompensate\<TActivity, TLog\>(CompensateActivityContext\<TActivity, TLog\>)**

Called after the message has been dispatched to all consumers - note that in the case of an exception
 this method is not called, and the DispatchFaulted method is called instead

```csharp
Task PostCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../masstransit/compensateactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CompensateFail\<TActivity, TLog\>(CompensateActivityContext\<TActivity, TLog\>, Exception)**

Called after the message has been dispatched to all consumers when one or more exceptions have occurred

```csharp
Task CompensateFail<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../masstransit/compensateactivitycontext-2)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
