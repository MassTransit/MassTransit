---

title: KeyProvider<TKey, TValue>

---

# KeyProvider\<TKey, TValue\>

Namespace: MassTransit.Internals.Caching

Returns the key for a value

```csharp
public sealed class KeyProvider<TKey, TValue> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TKey`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [KeyProvider\<TKey, TValue\>](../masstransit-internals-caching/keyprovider-2)<br/>
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

### **KeyProvider(Object, IntPtr)**

```csharp
public KeyProvider(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(TValue)**

```csharp
public TKey Invoke(TValue value)
```

#### Parameters

`value` TValue<br/>

#### Returns

TKey<br/>

### **BeginInvoke(TValue, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(TValue value, AsyncCallback callback, object object)
```

#### Parameters

`value` TValue<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public TKey EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

TKey<br/>
