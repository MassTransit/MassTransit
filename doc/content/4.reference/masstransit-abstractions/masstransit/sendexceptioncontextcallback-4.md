---

title: SendExceptionContextCallback<TSaga, TMessage, TException, T>

---

# SendExceptionContextCallback\<TSaga, TMessage, TException, T\>

Namespace: MassTransit

```csharp
public sealed class SendExceptionContextCallback<TSaga, TMessage, TException, T> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [SendExceptionContextCallback\<TSaga, TMessage, TException, T\>](../masstransit/sendexceptioncontextcallback-4)<br/>
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

### **SendExceptionContextCallback(Object, IntPtr)**

```csharp
public SendExceptionContextCallback(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(BehaviorExceptionContext\<TSaga, TMessage, TException\>, SendContext\<T\>)**

```csharp
public void Invoke(BehaviorExceptionContext<TSaga, TMessage, TException> context, SendContext<T> sendContext)
```

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>

`sendContext` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

### **BeginInvoke(BehaviorExceptionContext\<TSaga, TMessage, TException\>, SendContext\<T\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(BehaviorExceptionContext<TSaga, TMessage, TException> context, SendContext<T> sendContext, AsyncCallback callback, object object)
```

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>

`sendContext` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public void EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>
