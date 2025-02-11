---

title: RegistrationBusFactory

---

# RegistrationBusFactory

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class RegistrationBusFactory : IRegistrationBusFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationBusFactory](../masstransit-dependencyinjection-registration/registrationbusfactory)<br/>
Implements [IRegistrationBusFactory](../masstransit/iregistrationbusfactory)

## Constructors

### **RegistrationBusFactory(Func\<IBusRegistrationContext, IBusControl\>)**

```csharp
public RegistrationBusFactory(Func<IBusRegistrationContext, IBusControl> configure)
```

#### Parameters

`configure` [Func\<IBusRegistrationContext, IBusControl\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **CreateBus(IBusRegistrationContext, IEnumerable\<IBusInstanceSpecification\>, String)**

```csharp
public IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
```

#### Parameters

`context` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>

`specifications` [IEnumerable\<IBusInstanceSpecification\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`busName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IBusInstance](../masstransit-transports/ibusinstance)<br/>
