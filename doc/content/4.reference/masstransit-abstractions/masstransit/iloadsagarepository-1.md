---

title: ILoadSagaRepository<TSaga>

---

# ILoadSagaRepository\<TSaga\>

Namespace: MassTransit

```csharp
public interface ILoadSagaRepository<TSaga> : IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Load(Guid)**

```csharp
Task<TSaga> Load(Guid correlationId)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<TSaga\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
