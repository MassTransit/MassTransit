---

title: SupervisorExtensions

---

# SupervisorExtensions

Namespace: MassTransit

```csharp
public static class SupervisorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SupervisorExtensions](../masstransit/supervisorextensions)

## Methods

### **AddContext\<T\>(ISupervisor, T)**

Adds a context to the supervisor as an agent, which can be stopped by the supervisor.

```csharp
public static IPipeContextAgent<T> AddContext<T>(ISupervisor supervisor, T context)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`supervisor` [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor)<br/>
The supervisor

`context` T<br/>
The context

#### Returns

[IPipeContextAgent\<T\>](../masstransit-agents/ipipecontextagent-1)<br/>
A context handle

### **AddContext\<T\>(ISupervisor, Task\<T\>)**

Adds a context to the supervisor as an agent, which can be stopped by the supervisor.

```csharp
public static IPipeContextAgent<T> AddContext<T>(ISupervisor supervisor, Task<T> context)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`supervisor` [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor)<br/>
The supervisor

`context` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The context

#### Returns

[IPipeContextAgent\<T\>](../masstransit-agents/ipipecontextagent-1)<br/>
A context handle

### **AddActiveContext\<T\>(ISupervisor, PipeContextHandle\<T\>, Task\<T\>)**

Adds a context to the supervisor as an agent, which can be stopped by the supervisor.

```csharp
public static IActivePipeContextAgent<T> AddActiveContext<T>(ISupervisor supervisor, PipeContextHandle<T> contextHandle, Task<T> context)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`supervisor` [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor)<br/>
The supervisor

`contextHandle` [PipeContextHandle\<T\>](../masstransit/pipecontexthandle-1)<br/>
The actual context handle

`context` [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The active context

#### Returns

[IActivePipeContextAgent\<T\>](../masstransit-agents/iactivepipecontextagent-1)<br/>
A context handle

### **AddActiveContext\<T\>(ISupervisor, PipeContextHandle\<T\>, T)**

Adds a context to the supervisor as an agent, which can be stopped by the supervisor.

```csharp
public static IActivePipeContextAgent<T> AddActiveContext<T>(ISupervisor supervisor, PipeContextHandle<T> contextHandle, T context)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`supervisor` [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor)<br/>
The supervisor

`contextHandle` [PipeContextHandle\<T\>](../masstransit/pipecontexthandle-1)<br/>
The actual context handle

`context` T<br/>
The active context

#### Returns

[IActivePipeContextAgent\<T\>](../masstransit-agents/iactivepipecontextagent-1)<br/>
A context handle

### **AddAsyncContext\<T\>(ISupervisor)**

Adds a context to the supervisor as an agent, which can be stopped by the supervisor.

```csharp
public static IAsyncPipeContextAgent<T> AddAsyncContext<T>(ISupervisor supervisor)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`supervisor` [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor)<br/>
The supervisor

#### Returns

[IAsyncPipeContextAgent\<T\>](../masstransit-agents/iasyncpipecontextagent-1)<br/>
A context handle

### **CreateAgent\<T, TAgent\>(ISupervisor\<T\>, IAsyncPipeContextAgent\<TAgent\>, Func\<T, CancellationToken, Task\<TAgent\>\>, CancellationToken)**

```csharp
public static Task<TAgent> CreateAgent<T, TAgent>(ISupervisor<T> supervisor, IAsyncPipeContextAgent<TAgent> asyncContext, Func<T, CancellationToken, Task<TAgent>> agentFactory, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

`TAgent`<br/>

#### Parameters

`supervisor` [ISupervisor\<T\>](../../masstransit-abstractions/masstransit/isupervisor-1)<br/>

`asyncContext` [IAsyncPipeContextAgent\<TAgent\>](../masstransit-agents/iasyncpipecontextagent-1)<br/>

`agentFactory` [Func\<T, CancellationToken, Task\<TAgent\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<TAgent\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
