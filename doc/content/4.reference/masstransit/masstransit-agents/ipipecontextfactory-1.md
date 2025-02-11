---

title: IPipeContextFactory<TContext>

---

# IPipeContextFactory\<TContext\>

Namespace: MassTransit.Agents

Used to create the actual context, and the active context usages

```csharp
public interface IPipeContextFactory<TContext>
```

#### Type Parameters

`TContext`<br/>
The context type

## Methods

### **CreateContext(ISupervisor)**

Create the pipe context, which is the actual context, and not a copy of it

```csharp
IPipeContextAgent<TContext> CreateContext(ISupervisor supervisor)
```

#### Parameters

`supervisor` [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor)<br/>
The supervisor containing the context

#### Returns

[IPipeContextAgent\<TContext\>](../masstransit-agents/ipipecontextagent-1)<br/>
A handle to the pipe context

### **CreateActiveContext(ISupervisor, PipeContextHandle\<TContext\>, CancellationToken)**

Create an active pipe context, which is a reference to the actual context

```csharp
IActivePipeContextAgent<TContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<TContext> context, CancellationToken cancellationToken)
```

#### Parameters

`supervisor` [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor)<br/>
The supervisor containing the context

`context` [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1)<br/>
The actual context

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
The [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken) use for the active context

#### Returns

[IActivePipeContextAgent\<TContext\>](../masstransit-agents/iactivepipecontextagent-1)<br/>
A handle to the active context
