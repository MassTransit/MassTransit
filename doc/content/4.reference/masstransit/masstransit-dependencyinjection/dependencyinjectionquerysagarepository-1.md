---

title: DependencyInjectionQuerySagaRepository<TSaga>

---

# DependencyInjectionQuerySagaRepository\<TSaga\>

Namespace: MassTransit.DependencyInjection

```csharp
public class DependencyInjectionQuerySagaRepository<TSaga> : QuerySagaRepository<TSaga>, IQuerySagaRepository<TSaga>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [QuerySagaRepository\<TSaga\>](../masstransit-saga/querysagarepository-1) → [DependencyInjectionQuerySagaRepository\<TSaga\>](../masstransit-dependencyinjection/dependencyinjectionquerysagarepository-1)<br/>
Implements [IQuerySagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DependencyInjectionQuerySagaRepository(IServiceProvider)**

```csharp
public DependencyInjectionQuerySagaRepository(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>
