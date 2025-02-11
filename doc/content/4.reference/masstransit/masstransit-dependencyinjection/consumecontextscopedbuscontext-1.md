---

title: ConsumeContextScopedBusContext<TBus>

---

# ConsumeContextScopedBusContext\<TBus\>

Namespace: MassTransit.DependencyInjection

```csharp
public class ConsumeContextScopedBusContext<TBus> : ScopedBusContext
```

#### Type Parameters

`TBus`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextScopedBusContext\<TBus\>](../masstransit-dependencyinjection/consumecontextscopedbuscontext-1)<br/>
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

### **ConsumeContextScopedBusContext(TBus, ConsumeContext, IClientFactory, IServiceProvider)**

```csharp
public ConsumeContextScopedBusContext(TBus bus, ConsumeContext context, IClientFactory clientFactory, IServiceProvider provider)
```

#### Parameters

`bus` TBus<br/>

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`clientFactory` [IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory)<br/>

`provider` IServiceProvider<br/>
