---

title: InMemoryTestHarnessRegistrationBusFactory

---

# InMemoryTestHarnessRegistrationBusFactory

Namespace: MassTransit.Configuration

```csharp
public class InMemoryTestHarnessRegistrationBusFactory : IRegistrationBusFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryTestHarnessRegistrationBusFactory](../masstransit-configuration/inmemorytestharnessregistrationbusfactory)<br/>
Implements [IRegistrationBusFactory](../masstransit/iregistrationbusfactory)

## Constructors

### **InMemoryTestHarnessRegistrationBusFactory(String)**

```csharp
public InMemoryTestHarnessRegistrationBusFactory(string virtualHost)
```

#### Parameters

`virtualHost` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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
