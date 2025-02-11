---

title: IMediatorRegistrationConfigurator

---

# IMediatorRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface IMediatorRegistrationConfigurator : IRegistrationConfigurator, IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable
```

Implements [IRegistrationConfigurator](../masstransit/iregistrationconfigurator), IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Methods

### **ConfigureMediator(Action\<IMediatorRegistrationContext, IMediatorConfigurator\>)**

Optionally configure the pipeline used by the mediator

```csharp
void ConfigureMediator(Action<IMediatorRegistrationContext, IMediatorConfigurator> configure)
```

#### Parameters

`configure` [Action\<IMediatorRegistrationContext, IMediatorConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
