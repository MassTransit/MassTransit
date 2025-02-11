---

title: PipeExtensions

---

# PipeExtensions

Namespace: MassTransit

```csharp
public static class PipeExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PipeExtensions](../masstransit/pipeextensions)

## Methods

### **IsNotEmpty\<T\>(IPipe\<T\>)**

Returns true if the pipe is not empty

```csharp
public static bool IsNotEmpty<T>(IPipe<T> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<T\>](../masstransit/ipipe-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsEmpty\<T\>(IPipe\<T\>)**

Returns true if the pipe is empty

```csharp
public static bool IsEmpty<T>(IPipe<T> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<T\>](../masstransit/ipipe-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetPayload\<TPayload\>(PipeContext)**

Get a payload from the pipe context

```csharp
public static TPayload GetPayload<TPayload>(PipeContext context)
```

#### Type Parameters

`TPayload`<br/>
The payload type

#### Parameters

`context` [PipeContext](../masstransit/pipecontext)<br/>
The pipe context

#### Returns

TPayload<br/>
The payload, or throws a PayloadNotFoundException if the payload is not present

### **GetPayload\<TPayload\>(PipeContext, TPayload)**

Get a payload from the pipe context

```csharp
public static TPayload GetPayload<TPayload>(PipeContext context, TPayload defaultPayload)
```

#### Type Parameters

`TPayload`<br/>
The payload type

#### Parameters

`context` [PipeContext](../masstransit/pipecontext)<br/>
The pipe context

`defaultPayload` TPayload<br/>

#### Returns

TPayload<br/>
The payload, or the default Value

### **OneTimeSetup\<T\>(PipeContext, OneTimeSetupCallback)**

Using a filter-supplied context type, block so that the one time code is only executed once regardless of how many
 threads are pushing through the pipe at the same time.

```csharp
public static Task<OneTimeContext<T>> OneTimeSetup<T>(PipeContext context, OneTimeSetupCallback setupMethod)
```

#### Type Parameters

`T`<br/>
The payload type, should be an interface

#### Parameters

`context` [PipeContext](../masstransit/pipecontext)<br/>
The pipe context

`setupMethod` [OneTimeSetupCallback](../masstransit/onetimesetupcallback)<br/>
The setup method, called once regardless of the thread count

#### Returns

[Task\<OneTimeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
