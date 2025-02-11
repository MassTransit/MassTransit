---

title: LoadSagaRepositoryContext<TSaga>

---

# LoadSagaRepositoryContext\<TSaga\>

Namespace: MassTransit.Saga

```csharp
public interface LoadSagaRepositoryContext<TSaga> : PipeContext
```

#### Type Parameters

`TSaga`<br/>

Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Methods

### **Load(Guid)**

Load an existing saga instance

```csharp
Task<TSaga> Load(Guid correlationId)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<TSaga\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The saga, if found, or null
