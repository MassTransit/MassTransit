---

title: ScopedConsumePublishEndpointProvider

---

# ScopedConsumePublishEndpointProvider

Namespace: MassTransit.DependencyInjection

```csharp
public class ScopedConsumePublishEndpointProvider : IPublishEndpointProvider, IPublishObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedConsumePublishEndpointProvider](../masstransit-dependencyinjection/scopedconsumepublishendpointprovider)<br/>
Implements [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector)

## Constructors

### **ScopedConsumePublishEndpointProvider(IPublishEndpointProvider, ConsumeContext, IServiceProvider)**

```csharp
public ScopedConsumePublishEndpointProvider(IPublishEndpointProvider provider, ConsumeContext consumeContext, IServiceProvider serviceProvider)
```

#### Parameters

`provider` [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`serviceProvider` IServiceProvider<br/>
