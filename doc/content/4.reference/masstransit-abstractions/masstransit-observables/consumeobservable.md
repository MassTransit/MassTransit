---

title: ConsumeObservable

---

# ConsumeObservable

Namespace: MassTransit.Observables

```csharp
public class ConsumeObservable : Connectable<IConsumeObserver>, IConsumeObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConsumeObserver\>](../masstransit-util/connectable-1) → [ConsumeObservable](../masstransit-observables/consumeobservable)<br/>
Implements [IConsumeObserver](../masstransit/iconsumeobserver)

## Properties

### **Connected**

```csharp
public IConsumeObserver[] Connected { get; }
```

#### Property Value

[IConsumeObserver[]](../masstransit/iconsumeobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ConsumeObservable()**

```csharp
public ConsumeObservable()
```

## Methods

### **PreConsume\<T\>(ConsumeContext\<T\>)**

```csharp
public Task PreConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume\<T\>(ConsumeContext\<T\>)**

```csharp
public Task PostConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault\<T\>(ConsumeContext\<T\>, Exception)**

```csharp
public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
