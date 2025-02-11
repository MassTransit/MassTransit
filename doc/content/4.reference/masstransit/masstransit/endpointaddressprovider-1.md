---

title: EndpointAddressProvider<T>

---

# EndpointAddressProvider\<T\>

Namespace: MassTransit

Returns the address for the message provided

```csharp
public sealed class EndpointAddressProvider<T> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [EndpointAddressProvider\<T\>](../masstransit/endpointaddressprovider-1)<br/>
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

### **EndpointAddressProvider(Object, IntPtr)**

```csharp
public EndpointAddressProvider(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(Uri)**

```csharp
public bool Invoke(out Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **BeginInvoke(Uri, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(out Uri address, AsyncCallback callback, object object)
```

#### Parameters

`address` Uri<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(Uri, IAsyncResult)**

```csharp
public bool EndInvoke(out Uri address, IAsyncResult result)
```

#### Parameters

`address` Uri<br/>

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
