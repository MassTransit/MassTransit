---

title: MessageSchedulerFactory

---

# MessageSchedulerFactory

Namespace: MassTransit

```csharp
public sealed class MessageSchedulerFactory : MulticastDelegate, ICloneable, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [MessageSchedulerFactory](../masstransit/messageschedulerfactory)<br/>
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

### **MessageSchedulerFactory(Object, IntPtr)**

```csharp
public MessageSchedulerFactory(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(ConsumeContext)**

```csharp
public IMessageScheduler Invoke(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[IMessageScheduler](../masstransit/imessagescheduler)<br/>

### **BeginInvoke(ConsumeContext, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(ConsumeContext context, AsyncCallback callback, object object)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public IMessageScheduler EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[IMessageScheduler](../masstransit/imessagescheduler)<br/>
