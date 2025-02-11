---

title: PendingFutureIdProvider<T>

---

# PendingFutureIdProvider\<T\>

Namespace: MassTransit

```csharp
public sealed class PendingFutureIdProvider<T> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [PendingFutureIdProvider\<T\>](../masstransit/pendingfutureidprovider-1)<br/>
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

### **PendingFutureIdProvider(Object, IntPtr)**

```csharp
public PendingFutureIdProvider(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(T)**

```csharp
public Guid Invoke(T message)
```

#### Parameters

`message` T<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **BeginInvoke(T, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(T message, AsyncCallback callback, object object)
```

#### Parameters

`message` T<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public Guid EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
