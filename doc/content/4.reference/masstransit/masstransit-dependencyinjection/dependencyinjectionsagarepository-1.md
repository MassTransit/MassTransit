---

title: DependencyInjectionSagaRepository<TSaga>

---

# DependencyInjectionSagaRepository\<TSaga\>

Namespace: MassTransit.DependencyInjection

```csharp
public class DependencyInjectionSagaRepository<TSaga> : ISagaRepository<TSaga>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionSagaRepository\<TSaga\>](../masstransit-dependencyinjection/dependencyinjectionsagarepository-1)<br/>
Implements [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DependencyInjectionSagaRepository(IRegistrationContext)**

```csharp
public DependencyInjectionSagaRepository(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **DependencyInjectionSagaRepository(IServiceProvider, ISetScopedConsumeContext)**

```csharp
public DependencyInjectionSagaRepository(IServiceProvider serviceProvider, ISetScopedConsumeContext setter)
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

### **Send\<T\>(ConsumeContext\<T\>, ISagaPolicy\<TSaga, T\>, IPipe\<SagaConsumeContext\<TSaga, T\>\>)**

```csharp
public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`policy` [ISagaPolicy\<TSaga, T\>](../../masstransit-abstractions/masstransit/isagapolicy-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendQuery\<T\>(ConsumeContext\<T\>, ISagaQuery\<TSaga\>, ISagaPolicy\<TSaga, T\>, IPipe\<SagaConsumeContext\<TSaga, T\>\>)**

```csharp
public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

`policy` [ISagaPolicy\<TSaga, T\>](../../masstransit-abstractions/masstransit/isagapolicy-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
