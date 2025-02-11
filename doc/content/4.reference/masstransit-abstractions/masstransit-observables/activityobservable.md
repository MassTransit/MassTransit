---

title: ActivityObservable

---

# ActivityObservable

Namespace: MassTransit.Observables

```csharp
public class ActivityObservable : Connectable<IActivityObserver>, IActivityObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IActivityObserver\>](../masstransit-util/connectable-1) → [ActivityObservable](../masstransit-observables/activityobservable)<br/>
Implements [IActivityObserver](../masstransit/iactivityobserver)

## Properties

### **Connected**

```csharp
public IActivityObserver[] Connected { get; }
```

#### Property Value

[IActivityObserver[]](../masstransit/iactivityobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ActivityObservable()**

```csharp
public ActivityObservable()
```

## Methods

### **PreExecute\<TActivity, TArguments\>(ExecuteActivityContext\<TActivity, TArguments\>)**

```csharp
public Task PreExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../masstransit/executeactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostExecute\<TActivity, TArguments\>(ExecuteActivityContext\<TActivity, TArguments\>)**

```csharp
public Task PostExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../masstransit/executeactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ExecuteFault\<TActivity, TArguments\>(ExecuteActivityContext\<TActivity, TArguments\>, Exception)**

```csharp
public Task ExecuteFault<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception)
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

```csharp
public Task PreCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../masstransit/compensateactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostCompensate\<TActivity, TLog\>(CompensateActivityContext\<TActivity, TLog\>)**

```csharp
public Task PostCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../masstransit/compensateactivitycontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CompensateFail\<TActivity, TLog\>(CompensateActivityContext\<TActivity, TLog\>, Exception)**

```csharp
public Task CompensateFail<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../masstransit/compensateactivitycontext-2)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
