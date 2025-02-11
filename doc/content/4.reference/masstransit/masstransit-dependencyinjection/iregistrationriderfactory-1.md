---

title: IRegistrationRiderFactory<TRider>

---

# IRegistrationRiderFactory\<TRider\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface IRegistrationRiderFactory<TRider>
```

#### Type Parameters

`TRider`<br/>

## Methods

### **CreateRider(IRiderRegistrationContext)**

```csharp
IBusInstanceSpecification CreateRider(IRiderRegistrationContext context)
```

#### Parameters

`context` [IRiderRegistrationContext](../masstransit/iriderregistrationcontext)<br/>

#### Returns

[IBusInstanceSpecification](../masstransit-configuration/ibusinstancespecification)<br/>
