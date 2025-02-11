---

title: RetryPolicyFactory

---

# RetryPolicyFactory

Namespace: MassTransit.Configuration

```csharp
public sealed class RetryPolicyFactory : MulticastDelegate, ICloneable, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [RetryPolicyFactory](../masstransit-configuration/retrypolicyfactory)<br/>
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

### **RetryPolicyFactory(Object, IntPtr)**

```csharp
public RetryPolicyFactory(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(IExceptionFilter)**

```csharp
public IRetryPolicy Invoke(IExceptionFilter filter)
```

#### Parameters

`filter` [IExceptionFilter](../masstransit/iexceptionfilter)<br/>

#### Returns

[IRetryPolicy](../masstransit/iretrypolicy)<br/>

### **BeginInvoke(IExceptionFilter, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(IExceptionFilter filter, AsyncCallback callback, object object)
```

#### Parameters

`filter` [IExceptionFilter](../masstransit/iexceptionfilter)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public IRetryPolicy EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[IRetryPolicy](../masstransit/iretrypolicy)<br/>
