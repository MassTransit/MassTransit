---

title: FilterObservable<TContext>

---

# FilterObservable\<TContext\>

Namespace: MassTransit.Observables

```csharp
public class FilterObservable<TContext> : Connectable<IFilterObserver<TContext>>, IFilterObserver<TContext>
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IFilterObserver\<TContext\>\>](../masstransit-util/connectable-1) → [FilterObservable\<TContext\>](../masstransit-observables/filterobservable-1)<br/>
Implements [IFilterObserver\<TContext\>](../masstransit/ifilterobserver-1)

## Properties

### **Connected**

```csharp
public IFilterObserver`1[] Connected { get; }
```

#### Property Value

[IFilterObserver`1[]](../masstransit/ifilterobserver-1)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **FilterObservable()**

```csharp
public FilterObservable()
```

## Methods

### **PreSend(TContext)**

```csharp
public Task PreSend(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend(TContext)**

```csharp
public Task PostSend(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault(TContext, Exception)**

```csharp
public Task SendFault(TContext context, Exception exception)
```

#### Parameters

`context` TContext<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Method4()**

```csharp
public void Method4()
```

### **Method5()**

```csharp
public void Method5()
```

### **Method6()**

```csharp
public void Method6()
```
