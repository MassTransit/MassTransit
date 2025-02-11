---

title: ScopedBusContextProvider<TBus>

---

# ScopedBusContextProvider\<TBus\>

Namespace: MassTransit.DependencyInjection

Captures the bus context for the current scope as a scoped provider, so that it can be resolved
 by components at runtime (since MS DI doesn't support runtime configuration of scopes)

```csharp
public class ScopedBusContextProvider<TBus> : IScopedBusContextProvider<TBus>
```

#### Type Parameters

`TBus`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedBusContextProvider\<TBus\>](../masstransit-dependencyinjection/scopedbuscontextprovider-1)<br/>
Implements [IScopedBusContextProvider\<TBus\>](../masstransit-dependencyinjection/iscopedbuscontextprovider-1)

## Properties

### **Context**

```csharp
public ScopedBusContext Context { get; }
```

#### Property Value

[ScopedBusContext](../masstransit-dependencyinjection/scopedbuscontext)<br/>

## Constructors

### **ScopedBusContextProvider(TBus, Bind\<TBus, IClientFactory\>, Bind\<TBus, IScopedConsumeContextProvider\>, IScopedConsumeContextProvider, IServiceProvider)**

```csharp
public ScopedBusContextProvider(TBus bus, Bind<TBus, IClientFactory> clientFactory, Bind<TBus, IScopedConsumeContextProvider> busConsumeContextProvider, IScopedConsumeContextProvider globalConsumeContextProvider, IServiceProvider provider)
```

#### Parameters

`bus` TBus<br/>

`clientFactory` [Bind\<TBus, IClientFactory\>](../masstransit-dependencyinjection/bind-2)<br/>

`busConsumeContextProvider` [Bind\<TBus, IScopedConsumeContextProvider\>](../masstransit-dependencyinjection/bind-2)<br/>

`globalConsumeContextProvider` [IScopedConsumeContextProvider](../masstransit-dependencyinjection/iscopedconsumecontextprovider)<br/>

`provider` IServiceProvider<br/>
