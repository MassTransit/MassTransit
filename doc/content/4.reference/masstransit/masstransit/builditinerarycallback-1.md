---

title: BuildItineraryCallback<TInput>

---

# BuildItineraryCallback\<TInput\>

Namespace: MassTransit

Called by the future to build the routing slip

```csharp
public sealed class BuildItineraryCallback<TInput> : MulticastDelegate, ICloneable, ISerializable
```

#### Type Parameters

`TInput`<br/>
The input message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate) → [MulticastDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.multicastdelegate) → [BuildItineraryCallback\<TInput\>](../masstransit/builditinerarycallback-1)<br/>
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

### **BuildItineraryCallback(Object, IntPtr)**

```csharp
public BuildItineraryCallback(object object, IntPtr method)
```

#### Parameters

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`method` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br/>

## Methods

### **Invoke(BehaviorContext\<FutureState, TInput\>, IItineraryBuilder)**

```csharp
public Task Invoke(BehaviorContext<FutureState, TInput> context, IItineraryBuilder builder)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`builder` [IItineraryBuilder](../../masstransit-abstractions/masstransit/iitinerarybuilder)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **BeginInvoke(BehaviorContext\<FutureState, TInput\>, IItineraryBuilder, AsyncCallback, Object)**

```csharp
public IAsyncResult BeginInvoke(BehaviorContext<FutureState, TInput> context, IItineraryBuilder builder, AsyncCallback callback, object object)
```

#### Parameters

`context` [BehaviorContext\<FutureState, TInput\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`builder` [IItineraryBuilder](../../masstransit-abstractions/masstransit/iitinerarybuilder)<br/>

`callback` [AsyncCallback](https://learn.microsoft.com/en-us/dotnet/api/system.asynccallback)<br/>

`object` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

### **EndInvoke(IAsyncResult)**

```csharp
public Task EndInvoke(IAsyncResult result)
```

#### Parameters

`result` [IAsyncResult](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncresult)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
