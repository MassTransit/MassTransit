---

title: ConsumeMessageObservable<T>

---

# ConsumeMessageObservable\<T\>

Namespace: MassTransit.Observables

```csharp
public class ConsumeMessageObservable<T> : Connectable<IConsumeMessageObserver<T>>, IConsumeMessageObserver<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConsumeMessageObserver\<T\>\>](../masstransit-util/connectable-1) → [ConsumeMessageObservable\<T\>](../masstransit-observables/consumemessageobservable-1)<br/>
Implements [IConsumeMessageObserver\<T\>](../masstransit/iconsumemessageobserver-1)

## Properties

### **Connected**

```csharp
public IConsumeMessageObserver`1[] Connected { get; }
```

#### Property Value

[IConsumeMessageObserver`1[]](../masstransit/iconsumemessageobserver-1)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ConsumeMessageObservable()**

```csharp
public ConsumeMessageObservable()
```

## Methods

### **PreConsume(ConsumeContext\<T\>)**

```csharp
public Task PreConsume(ConsumeContext<T> context)
```

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume(ConsumeContext\<T\>)**

```csharp
public Task PostConsume(ConsumeContext<T> context)
```

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault(ConsumeContext\<T\>, Exception)**

```csharp
public Task ConsumeFault(ConsumeContext<T> context, Exception exception)
```

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
