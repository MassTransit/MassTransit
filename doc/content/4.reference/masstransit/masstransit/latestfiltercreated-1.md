---

title: LatestFilterCreated<T>

---

# LatestFilterCreated\<T\>

Namespace: MassTransit

```csharp
public sealed class LatestFilterCreated<T> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [LatestFilterCreated\<T\>](../masstransit/latestfiltercreated-1)<br/>
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

### **LatestFilterCreated(Object, IntPtr)**

```csharp
public LatestFilterCreated(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(ILatestFilter\<T\>)**

```csharp
public void Invoke(ILatestFilter<T> filter)
```

#### Parameters

`filter` [ILatestFilter\<T\>](../masstransit-middleware/ilatestfilter-1)<br/>

### **BeginInvoke(ILatestFilter\<T\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(ILatestFilter<T> filter, AsyncCallback callback, object object)
```

#### Parameters

`filter` [ILatestFilter\<T\>](../masstransit-middleware/ilatestfilter-1)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public void EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>
