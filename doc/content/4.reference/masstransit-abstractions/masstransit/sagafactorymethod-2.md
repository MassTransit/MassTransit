---

title: SagaFactoryMethod<TSaga, TMessage>

---

# SagaFactoryMethod\<TSaga, TMessage\>

Namespace: MassTransit

Used to create the saga instance

```csharp
public sealed class SagaFactoryMethod<TSaga, TMessage> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TSaga`<br/>
The saga type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [SagaFactoryMethod\<TSaga, TMessage\>](../masstransit/sagafactorymethod-2)<br/>
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

### **SagaFactoryMethod(Object, IntPtr)**

```csharp
public SagaFactoryMethod(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(ConsumeContext\<TMessage\>)**

```csharp
public TSaga Invoke(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>

#### Returns

TSaga<br/>

### **BeginInvoke(ConsumeContext\<TMessage\>, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(ConsumeContext<TMessage> context, AsyncCallback callback, object object)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public TSaga EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

TSaga<br/>
