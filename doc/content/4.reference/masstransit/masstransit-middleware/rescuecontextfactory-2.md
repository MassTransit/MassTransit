---

title: RescueContextFactory<TContext, TRescueContext>

---

# RescueContextFactory\<TContext, TRescueContext\>

Namespace: MassTransit.Middleware

```csharp
public sealed class RescueContextFactory<TContext, TRescueContext> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TContext`<br/>

`TRescueContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [RescueContextFactory\<TContext, TRescueContext\>](../masstransit-middleware/rescuecontextfactory-2)<br/>
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

### **RescueContextFactory(Object, IntPtr)**

```csharp
public RescueContextFactory(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(TContext, Exception)**

```csharp
public TRescueContext Invoke(TContext context, Exception exception)
```

#### Parameters

`context` TContext<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

TRescueContext<br/>

### **BeginInvoke(TContext, Exception, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(TContext context, Exception exception, AsyncCallback callback, object object)
```

#### Parameters

`context` TContext<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public TRescueContext EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

TRescueContext<br/>
