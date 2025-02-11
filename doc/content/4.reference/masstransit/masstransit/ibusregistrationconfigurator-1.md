---

title: IBusRegistrationConfigurator<TBus>

---

# IBusRegistrationConfigurator\<TBus\>

Namespace: MassTransit

Configures additional bus instances, configured via MultiBus

```csharp
public interface IBusRegistrationConfigurator<TBus> : IBusRegistrationConfigurator, IRegistrationConfigurator, IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable
```

#### Type Parameters

`TBus`<br/>
The additional bus interface type

Implements [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator), [IRegistrationConfigurator](../masstransit/iregistrationconfigurator), IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Methods

### **AddRider(Action\<IRiderRegistrationConfigurator\<TBus\>\>)**

Add bus rider

```csharp
void AddRider(Action<IRiderRegistrationConfigurator<TBus>> configure)
```

#### Parameters

`configure` [Action\<IRiderRegistrationConfigurator\<TBus\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
