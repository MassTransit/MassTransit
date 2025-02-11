---

title: KeyAccessor<TContext, TKey>

---

# KeyAccessor\<TContext, TKey\>

Namespace: MassTransit.Middleware

```csharp
public sealed class KeyAccessor<TContext, TKey> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TContext`<br/>

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [KeyAccessor\<TContext, TKey\>](../masstransit-middleware/keyaccessor-2)<br/>
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

### **KeyAccessor(Object, IntPtr)**

```csharp
public KeyAccessor(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(TContext)**

```csharp
public TKey Invoke(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

TKey<br/>

### **BeginInvoke(TContext, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(TContext context, AsyncCallback callback, object object)
```

#### Parameters

`context` TContext<br/>

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
