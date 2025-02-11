---

title: BusTestConsumeObserver

---

# BusTestConsumeObserver

Namespace: MassTransit.Testing.Implementations

```csharp
public class BusTestConsumeObserver : InactivityTestObserver, IDisposable, IInactivityObservationSource, IConsumeObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IInactivityObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [InactivityTestObserver](../masstransit-testing-implementations/inactivitytestobserver) → [BusTestConsumeObserver](../masstransit-testing-implementations/bustestconsumeobserver)<br/>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [IInactivityObservationSource](../masstransit-testing-implementations/iinactivityobservationsource), [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)

## Properties

### **Messages**

```csharp
public IReceivedMessageList Messages { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

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

### **BusTestConsumeObserver(TimeSpan, CancellationToken)**

```csharp
public BusTestConsumeObserver(TimeSpan timeout, CancellationToken testCompleted)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`testCompleted` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **PreConsume\<T\>(ConsumeContext\<T\>)**

```csharp
public Task PreConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume\<T\>(ConsumeContext\<T\>)**

```csharp
public Task PostConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault\<T\>(ConsumeContext\<T\>, Exception)**

```csharp
public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
