---

title: MissingValueFactory<TKey, TValue>

---

# MissingValueFactory\<TKey, TValue\>

Namespace: MassTransit.Caching

Creates the value if it is not found in the index

```csharp
public sealed class MissingValueFactory<TKey, TValue> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TKey`<br/>
The key type

`TValue`<br/>
The value type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [MissingValueFactory\<TKey, TValue\>](../masstransit-caching/missingvaluefactory-2)<br/>
Implements [ICloneable](https://learn.microsoft.com/en-us/dotnet/api/system.icloneable), [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)

## Properties

### **Target**

```csharp
public object Target { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **Method**

```csharp
public MethodInfo Method { get; }
```

#### Property Value

[MethodInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo)<br/>

## Constructors

### **MissingValueFactory(Object, IntPtr)**

```csharp
public MissingValueFactory(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(TKey)**

```csharp
public Task<TValue> Invoke(TKey key)
```

#### Parameters

`key` TKey<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **BeginInvoke(TKey, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(TKey key, AsyncCallback callback, object object)
```

#### Parameters

`key` TKey<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public Task<TValue> EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
