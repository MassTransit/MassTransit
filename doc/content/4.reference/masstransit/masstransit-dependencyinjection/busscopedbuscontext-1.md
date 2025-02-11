---

title: BusScopedBusContext<TBus>

---

# BusScopedBusContext\<TBus\>

Namespace: MassTransit.DependencyInjection

```csharp
public class BusScopedBusContext<TBus> : ScopedBusContext
```

#### Type Parameters

`TBus`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusScopedBusContext\<TBus\>](../masstransit-dependencyinjection/busscopedbuscontext-1)<br/>
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

### **BusScopedBusContext(TBus, IClientFactory, IServiceProvider)**

```csharp
public BusScopedBusContext(TBus bus, IClientFactory clientFactory, IServiceProvider provider)
```

#### Parameters

`bus` TBus<br/>

`clientFactory` [IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory)<br/>

`provider` IServiceProvider<br/>
