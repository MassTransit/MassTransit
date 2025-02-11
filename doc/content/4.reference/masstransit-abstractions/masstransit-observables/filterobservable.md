---

title: FilterObservable

---

# FilterObservable

Namespace: MassTransit.Observables

```csharp
public class FilterObservable : Connectable<IFilterObserver>, IFilterObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IFilterObserver\>](../masstransit-util/connectable-1) → [FilterObservable](../masstransit-observables/filterobservable)<br/>
Implements [IFilterObserver](../masstransit/ifilterobserver)

## Properties

### **Connected**

```csharp
public IFilterObserver[] Connected { get; }
```

#### Property Value

[IFilterObserver[]](../masstransit/ifilterobserver)<br/>

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

### **PreSend\<T\>(T)**

```csharp
public Task PreSend<T>(T context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend\<T\>(T)**

```csharp
public Task PostSend<T>(T context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault\<T\>(T, Exception)**

```csharp
public Task SendFault<T>(T context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` T<br/>

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
