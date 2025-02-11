---

title: BusTestReceiveObserver

---

# BusTestReceiveObserver

Namespace: MassTransit.Testing.Implementations

```csharp
public class BusTestReceiveObserver : InactivityTestObserver, IDisposable, IInactivityObservationSource, IReceiveObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IInactivityObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [InactivityTestObserver](../masstransit-testing-implementations/inactivitytestobserver) → [BusTestReceiveObserver](../masstransit-testing-implementations/bustestreceiveobserver)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IInactivityObservationSource](../masstransit-testing-implementations/iinactivityobservationsource), [IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)

## Properties

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

### **BusTestReceiveObserver(TimeSpan)**

```csharp
public BusTestReceiveObserver(TimeSpan inactivityTimout)
```

#### Parameters

`inactivityTimout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **PreReceive(ReceiveContext)**

```csharp
public Task PreReceive(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostReceive(ReceiveContext)**

```csharp
public Task PostReceive(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume\<T\>(ConsumeContext\<T\>, TimeSpan, String)**

```csharp
public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault\<T\>(ConsumeContext\<T\>, TimeSpan, String, Exception)**

```csharp
public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ReceiveFault(ReceiveContext, Exception)**

```csharp
public Task ReceiveFault(ReceiveContext context, Exception exception)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
