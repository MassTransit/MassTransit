---

title: AllowTransportHeader

---

# AllowTransportHeader

Namespace: MassTransit.Transports

```csharp
public sealed class AllowTransportHeader : MulticastDelegate, ICloneable, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [AllowTransportHeader](../masstransit-transports/allowtransportheader)<br/>
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

### **AllowTransportHeader(Object, IntPtr)**

```csharp
public AllowTransportHeader(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(HeaderValue\<String\>)**

```csharp
public bool Invoke(HeaderValue<string> headerValue)
```

#### Parameters

`headerValue` [HeaderValue\<String\>](../../masstransit-abstractions/masstransit/headervalue-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **BeginInvoke(HeaderValue\<String\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(HeaderValue<string> headerValue, AsyncCallback callback, object object)
```

#### Parameters

`headerValue` [HeaderValue\<String\>](../../masstransit-abstractions/masstransit/headervalue-1)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public bool EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
