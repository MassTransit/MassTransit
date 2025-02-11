---

title: SagaFilterFactory<TInstance, TData>

---

# SagaFilterFactory\<TInstance, TData\>

Namespace: MassTransit

```csharp
public sealed class SagaFilterFactory<TInstance, TData> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [SagaFilterFactory\<TInstance, TData\>](../masstransit/sagafilterfactory-2)<br/>
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

### **SagaFilterFactory(Object, IntPtr)**

```csharp
public SagaFilterFactory(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(ISagaRepository\<TInstance\>, ISagaPolicy\<TInstance, TData\>, IPipe\<SagaConsumeContext\<TInstance, TData\>\>)**

```csharp
public IFilter<ConsumeContext<TData>> Invoke(ISagaRepository<TInstance> repository, ISagaPolicy<TInstance, TData> policy, IPipe<SagaConsumeContext<TInstance, TData>> sagaPipe)
```

#### Parameters

`repository` [ISagaRepository\<TInstance\>](../masstransit/isagarepository-1)<br/>

`policy` [ISagaPolicy\<TInstance, TData\>](../masstransit/isagapolicy-2)<br/>

`sagaPipe` [IPipe\<SagaConsumeContext\<TInstance, TData\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[IFilter\<ConsumeContext\<TData\>\>](../masstransit/ifilter-1)<br/>

### **BeginInvoke(ISagaRepository\<TInstance\>, ISagaPolicy\<TInstance, TData\>, IPipe\<SagaConsumeContext\<TInstance, TData\>\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(ISagaRepository<TInstance> repository, ISagaPolicy<TInstance, TData> policy, IPipe<SagaConsumeContext<TInstance, TData>> sagaPipe, AsyncCallback callback, object object)
```

#### Parameters

`repository` [ISagaRepository\<TInstance\>](../masstransit/isagarepository-1)<br/>

`policy` [ISagaPolicy\<TInstance, TData\>](../masstransit/isagapolicy-2)<br/>

`sagaPipe` [IPipe\<SagaConsumeContext\<TInstance, TData\>\>](../masstransit/ipipe-1)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public IFilter<ConsumeContext<TData>> EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[IFilter\<ConsumeContext\<TData\>\>](../masstransit/ifilter-1)<br/>
