---

title: TransactionalScopedBusContextProvider<TBus>

---

# TransactionalScopedBusContextProvider\<TBus\>

Namespace: MassTransit.DependencyInjection

```csharp
public class TransactionalScopedBusContextProvider<TBus> : IScopedBusContextProvider<TBus>
```

#### Type Parameters

`TBus`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransactionalScopedBusContextProvider\<TBus\>](../masstransit-dependencyinjection/transactionalscopedbuscontextprovider-1)<br/>
Implements [IScopedBusContextProvider\<TBus\>](../masstransit-dependencyinjection/iscopedbuscontextprovider-1)

## Properties

### **Context**

```csharp
public ScopedBusContext Context { get; }
```

#### Property Value

[ScopedBusContext](../masstransit-dependencyinjection/scopedbuscontext)<br/>

## Constructors

### **TransactionalScopedBusContextProvider(ITransactionalBus, Bind\<TBus, IClientFactory\>, Bind\<TBus, IScopedConsumeContextProvider\>, IScopedConsumeContextProvider, IServiceProvider)**

```csharp
public TransactionalScopedBusContextProvider(ITransactionalBus bus, Bind<TBus, IClientFactory> clientFactory, Bind<TBus, IScopedConsumeContextProvider> consumeContextProvider, IScopedConsumeContextProvider globalConsumeContextProvider, IServiceProvider provider)
```

#### Parameters

`bus` [ITransactionalBus](../masstransit-transactions/itransactionalbus)<br/>

`clientFactory` [Bind\<TBus, IClientFactory\>](../masstransit-dependencyinjection/bind-2)<br/>

`consumeContextProvider` [Bind\<TBus, IScopedConsumeContextProvider\>](../masstransit-dependencyinjection/bind-2)<br/>

`globalConsumeContextProvider` [IScopedConsumeContextProvider](../masstransit-dependencyinjection/iscopedconsumecontextprovider)<br/>

`provider` IServiceProvider<br/>
