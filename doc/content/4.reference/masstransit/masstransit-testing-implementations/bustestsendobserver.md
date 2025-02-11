---

title: BusTestSendObserver

---

# BusTestSendObserver

Namespace: MassTransit.Testing.Implementations

```csharp
public class BusTestSendObserver : InactivityTestObserver, IDisposable, IInactivityObservationSource, ISendObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IInactivityObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [InactivityTestObserver](../masstransit-testing-implementations/inactivitytestobserver) → [BusTestSendObserver](../masstransit-testing-implementations/bustestsendobserver)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IInactivityObservationSource](../masstransit-testing-implementations/iinactivityobservationsource), [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)

## Properties

### **Messages**

```csharp
public ISentMessageList Messages { get; }
```

#### Property Value

[ISentMessageList](../masstransit-testing/isentmessagelist)<br/>

### **IsInactive**

```csharp
public bool IsInactive { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Connected**

```csharp
public IInactivityObserver[] Connected { get; }
```

#### Property Value

[IInactivityObserver[]](../masstransit-testing-implementations/iinactivityobserver)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **BusTestSendObserver(TimeSpan, TimeSpan, CancellationToken)**

```csharp
public BusTestSendObserver(TimeSpan timeout, TimeSpan inactivityTimout, CancellationToken testCompleted)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`inactivityTimout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`testCompleted` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **PreSend\<T\>(SendContext\<T\>)**

```csharp
public Task PreSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend\<T\>(SendContext\<T\>)**

```csharp
public Task PostSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault\<T\>(SendContext\<T\>, Exception)**

```csharp
public Task SendFault<T>(SendContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
