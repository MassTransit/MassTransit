---

title: RequestAddressProvider<TMessage>

---

# RequestAddressProvider\<TMessage\>

Namespace: MassTransit

```csharp
public sealed class RequestAddressProvider<TMessage> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [RequestAddressProvider\<TMessage\>](../masstransit/requestaddressprovider-1)<br/>
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

### **RequestAddressProvider(Object, IntPtr)**

```csharp
public RequestAddressProvider(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(BehaviorContext\<FutureState, TMessage\>)**

```csharp
public Uri Invoke(BehaviorContext<FutureState, TMessage> context)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TMessage\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

Uri<br/>

### **BeginInvoke(BehaviorContext\<FutureState, TMessage\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(BehaviorContext<FutureState, TMessage> context, AsyncCallback callback, object object)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TMessage\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public Uri EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

Uri<br/>
