---

title: IRegistrationBusFactory

---

# IRegistrationBusFactory

Namespace: MassTransit

```csharp
public interface IRegistrationBusFactory
```

## Methods

### **CreateBus(IBusRegistrationContext, IEnumerable\<IBusInstanceSpecification\>, String)**

```csharp
IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
```

#### Parameters

`context` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>

`specifications` [IEnumerable\<IBusInstanceSpecification\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`busName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IBusInstance](../masstransit-transports/ibusinstance)<br/>
