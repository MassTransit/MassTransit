---

title: SendObservable

---

# SendObservable

Namespace: MassTransit.Observables

```csharp
public class SendObservable : Connectable<ISendObserver>, ISendObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<ISendObserver\>](../masstransit-util/connectable-1) → [SendObservable](../masstransit-observables/sendobservable)<br/>
Implements [ISendObserver](../masstransit/isendobserver)

## Properties

### **Connected**

```csharp
public ISendObserver[] Connected { get; }
```

#### Property Value

[ISendObserver[]](../masstransit/isendobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **SendObservable()**

```csharp
public SendObservable()
```

## Methods

### **PreSend\<T\>(SendContext\<T\>)**

```csharp
public Task PreSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend\<T\>(SendContext\<T\>)**

```csharp
public Task PostSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault\<T\>(SendContext\<T\>, Exception)**

```csharp
public Task SendFault<T>(SendContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
