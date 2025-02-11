---

title: SendEndpointFactory<TKey>

---

# SendEndpointFactory\<TKey\>

Namespace: MassTransit.Transports

Factory method for a send endpoint

```csharp
public sealed class SendEndpointFactory<TKey> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [SendEndpointFactory\<TKey\>](../masstransit-transports/sendendpointfactory-1)<br/>
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

### **SendEndpointFactory(Object, IntPtr)**

```csharp
public SendEndpointFactory(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(TKey)**

```csharp
public Task<ISendEndpoint> Invoke(TKey key)
```

#### Parameters

`key` TKey<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **BeginInvoke(TKey, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(TKey key, AsyncCallback callback, object object)
```

#### Parameters

`key` TKey<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public Task<ISendEndpoint> EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
