---

title: IRiderRegistrationContext

---

# IRiderRegistrationContext

Namespace: MassTransit

```csharp
public interface IRiderRegistrationContext : IRegistrationContext, IServiceProvider
```

Implements [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext), IServiceProvider

## Methods

### **GetRegistrations\<T\>()**

```csharp
IEnumerable<T> GetRegistrations<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
