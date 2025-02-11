---

title: DependencyInjectionSagaRepositoryContextFactory<TSaga>

---

# DependencyInjectionSagaRepositoryContextFactory\<TSaga\>

Namespace: MassTransit.DependencyInjection

```csharp
public class DependencyInjectionSagaRepositoryContextFactory<TSaga> : ISagaRepositoryContextFactory<TSaga>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionSagaRepositoryContextFactory\<TSaga\>](../masstransit-dependencyinjection/dependencyinjectionsagarepositorycontextfactory-1)<br/>
Implements [ISagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/isagarepositorycontextfactory-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DependencyInjectionSagaRepositoryContextFactory(IRegistrationContext)**

```csharp
public DependencyInjectionSagaRepositoryContextFactory(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **DependencyInjectionSagaRepositoryContextFactory(IServiceProvider, ISetScopedConsumeContext)**

```csharp
public DependencyInjectionSagaRepositoryContextFactory(IServiceProvider serviceProvider, ISetScopedConsumeContext setter)
```

#### Parameters

`serviceProvider` IServiceProvider<br/>

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send\<T\>(ConsumeContext\<T\>, IPipe\<SagaRepositoryContext\<TSaga, T\>\>)**

```csharp
public Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<SagaRepositoryContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendQuery\<T\>(ConsumeContext\<T\>, ISagaQuery\<TSaga\>, IPipe\<SagaRepositoryQueryContext\<TSaga, T\>\>)**

```csharp
public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

`next` [IPipe\<SagaRepositoryQueryContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
