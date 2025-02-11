---

title: ObservableObserver<T>

---

# ObservableObserver\<T\>

Namespace: MassTransit.Util

```csharp
public class ObservableObserver<T> : IObservable<T>, IObserver<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ObservableObserver\<T\>](../masstransit-util/observableobserver-1)<br/>
Implements [IObservable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobservable-1), [IObserver\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)

## Constructors

### **ObservableObserver()**

```csharp
public ObservableObserver()
```

## Methods

### **Subscribe(IObserver\<T\>)**

```csharp
public IDisposable Subscribe(IObserver<T> observer)
```

#### Parameters

`observer` [IObserver\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

### **OnNext(T)**

```csharp
public void OnNext(T value)
```

#### Parameters

`value` T<br/>

### **OnError(Exception)**

```csharp
public void OnError(Exception error)
```

#### Parameters

`error` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **OnCompleted()**

```csharp
public void OnCompleted()
```
