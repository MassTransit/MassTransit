---

title: ScopedConsumeSendEndpointProvider

---

# ScopedConsumeSendEndpointProvider

Namespace: MassTransit.DependencyInjection

```csharp
public class ScopedConsumeSendEndpointProvider : ISendEndpointProvider, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedConsumeSendEndpointProvider](../masstransit-dependencyinjection/scopedconsumesendendpointprovider)<br/>
Implements [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Constructors

### **ScopedConsumeSendEndpointProvider(ISendEndpointProvider, ConsumeContext, IServiceProvider)**

```csharp
public ScopedConsumeSendEndpointProvider(ISendEndpointProvider provider, ConsumeContext consumeContext, IServiceProvider scope)
```

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`scope` IServiceProvider<br/>
