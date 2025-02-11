---

title: ScopedSendEndpointProvider

---

# ScopedSendEndpointProvider

Namespace: MassTransit.DependencyInjection

```csharp
public class ScopedSendEndpointProvider : ISendEndpointProvider, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedSendEndpointProvider](../masstransit-dependencyinjection/scopedsendendpointprovider)<br/>
Implements [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Constructors

### **ScopedSendEndpointProvider(ISendEndpointProvider, IServiceProvider)**

```csharp
public ScopedSendEndpointProvider(ISendEndpointProvider provider, IServiceProvider serviceProvider)
```

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`serviceProvider` IServiceProvider<br/>
