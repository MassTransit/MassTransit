---

title: ConsumeContextScopedBusContext

---

# ConsumeContextScopedBusContext

Namespace: MassTransit.DependencyInjection

```csharp
public class ConsumeContextScopedBusContext : ScopedBusContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextScopedBusContext](../masstransit-dependencyinjection/consumecontextscopedbuscontext)<br/>
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

### **ConsumeContextScopedBusContext(ConsumeContext, IClientFactory)**

```csharp
public ConsumeContextScopedBusContext(ConsumeContext context, IClientFactory clientFactory)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`clientFactory` [IClientFactory](../../masstransit-abstractions/masstransit/iclientfactory)<br/>
