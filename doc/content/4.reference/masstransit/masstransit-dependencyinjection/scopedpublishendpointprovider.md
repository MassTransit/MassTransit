---

title: ScopedPublishEndpointProvider

---

# ScopedPublishEndpointProvider

Namespace: MassTransit.DependencyInjection

```csharp
public class ScopedPublishEndpointProvider : IPublishEndpointProvider, IPublishObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedPublishEndpointProvider](../masstransit-dependencyinjection/scopedpublishendpointprovider)<br/>
Implements [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector)

## Constructors

### **ScopedPublishEndpointProvider(IPublishEndpointProvider, IServiceProvider)**

```csharp
public ScopedPublishEndpointProvider(IPublishEndpointProvider provider, IServiceProvider serviceProvider)
```

#### Parameters

`provider` [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

`serviceProvider` IServiceProvider<br/>
