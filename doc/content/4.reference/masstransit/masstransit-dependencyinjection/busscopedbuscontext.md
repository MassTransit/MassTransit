---

title: BusScopedBusContext

---

# BusScopedBusContext

Namespace: MassTransit.DependencyInjection

```csharp
public class BusScopedBusContext : ScopedBusContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusScopedBusContext](../masstransit-dependencyinjection/busscopedbuscontext)<br/>
Implements [ScopedBusContext](../masstransit-dependencyinjection/scopedbuscontext)

## Properties

### **SendEndpointProvider**

```csharp
public ISendEndpointProvider SendEndpointProvider { get; }
```

#### Property Value

[ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

### **PublishEndpoint**

```csharp
public IPublishEndpoint PublishEndpoint { get; }
```

#### Property Value

[IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

### **ClientFactory**

```csharp
public IScopedClientFactory ClientFactory { get; }
```

#### Property Value

[IScopedClientFactory](../masstransit/iscopedclientfactory)<br/>

## Constructors

### **BusScopedBusContext(ScopedBusContext, IClientFactory, IServiceProvider)**

```csharp
public BusScopedBusContext(ScopedBusContext scopedBusContext, IClientFactory clientFactory, IServiceProvider provider)
```

#### Parameters

`scopedBusContext` [ScopedBusContext](../masstransit-dependencyinjection/scopedbuscontext)<br/>

`clientFactory` [IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory)<br/>

`provider` IServiceProvider<br/>
