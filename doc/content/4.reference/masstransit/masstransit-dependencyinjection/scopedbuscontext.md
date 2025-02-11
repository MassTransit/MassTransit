---

title: ScopedBusContext

---

# ScopedBusContext

Namespace: MassTransit.DependencyInjection

```csharp
public interface ScopedBusContext
```

## Properties

### **SendEndpointProvider**

```csharp
public abstract ISendEndpointProvider SendEndpointProvider { get; }
```

#### Property Value

[ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

### **PublishEndpoint**

```csharp
public abstract IPublishEndpoint PublishEndpoint { get; }
```

#### Property Value

[IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

### **ClientFactory**

```csharp
public abstract IScopedClientFactory ClientFactory { get; }
```

#### Property Value

[IScopedClientFactory](../masstransit/iscopedclientfactory)<br/>
