---

title: LoadBalancerFactory<T>

---

# LoadBalancerFactory\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public sealed class LoadBalancerFactory<T> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [LoadBalancerFactory\<T\>](../masstransit-transports-fabric/loadbalancerfactory-1)<br/>
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

### **LoadBalancerFactory(Object, IntPtr)**

```csharp
public LoadBalancerFactory(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(IMessageReceiver`1[])**

```csharp
public IReceiverLoadBalancer<T> Invoke(IMessageReceiver`1[] consumers)
```

#### Parameters

`consumers` [IMessageReceiver`1[]](../masstransit-transports-fabric/imessagereceiver-1)<br/>

#### Returns

[IReceiverLoadBalancer\<T\>](../masstransit-transports-fabric/ireceiverloadbalancer-1)<br/>

### **BeginInvoke(IMessageReceiver`1[], AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(IMessageReceiver`1[] consumers, AsyncCallback callback, object object)
```

#### Parameters

`consumers` [IMessageReceiver`1[]](../masstransit-transports-fabric/imessagereceiver-1)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public IReceiverLoadBalancer<T> EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[IReceiverLoadBalancer\<T\>](../masstransit-transports-fabric/ireceiverloadbalancer-1)<br/>
