---

title: ReceiveObservable

---

# ReceiveObservable

Namespace: MassTransit.Observables

```csharp
public class ReceiveObservable : Connectable<IReceiveObserver>, IReceiveObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IReceiveObserver\>](../masstransit-util/connectable-1) → [ReceiveObservable](../masstransit-observables/receiveobservable)<br/>
Implements [IReceiveObserver](../masstransit/ireceiveobserver)

## Properties

### **Connected**

```csharp
public IReceiveObserver[] Connected { get; }
```

#### Property Value

[IReceiveObserver[]](../masstransit/ireceiveobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ReceiveObservable()**

```csharp
public ReceiveObservable()
```

## Methods

### **PreReceive(ReceiveContext)**

```csharp
public Task PreReceive(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostReceive(ReceiveContext)**

```csharp
public Task PostReceive(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume\<T\>(ConsumeContext\<T\>, TimeSpan, String)**

```csharp
public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

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

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

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

`context` [ReceiveContext](../masstransit/receivecontext)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
