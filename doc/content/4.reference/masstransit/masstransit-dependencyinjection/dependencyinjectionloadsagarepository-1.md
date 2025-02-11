---

title: DependencyInjectionLoadSagaRepository<TSaga>

---

# DependencyInjectionLoadSagaRepository\<TSaga\>

Namespace: MassTransit.DependencyInjection

```csharp
public class DependencyInjectionLoadSagaRepository<TSaga> : LoadSagaRepository<TSaga>, ILoadSagaRepository<TSaga>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LoadSagaRepository\<TSaga\>](../masstransit-saga/loadsagarepository-1) → [DependencyInjectionLoadSagaRepository\<TSaga\>](../masstransit-dependencyinjection/dependencyinjectionloadsagarepository-1)<br/>
Implements [ILoadSagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DependencyInjectionLoadSagaRepository(IServiceProvider)**

```csharp
public DependencyInjectionLoadSagaRepository(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>
