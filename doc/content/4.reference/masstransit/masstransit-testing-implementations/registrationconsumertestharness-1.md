---

title: RegistrationConsumerTestHarness<TConsumer>

---

# RegistrationConsumerTestHarness\<TConsumer\>

Namespace: MassTransit.Testing.Implementations

```csharp
public class RegistrationConsumerTestHarness<TConsumer> : IConsumerTestHarness<TConsumer>
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationConsumerTestHarness\<TConsumer\>](../masstransit-testing-implementations/registrationconsumertestharness-1)<br/>
Implements [IConsumerTestHarness\<TConsumer\>](../masstransit-testing/iconsumertestharness-1)

## Properties

### **Consumed**

```csharp
public IReceivedMessageList Consumed { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

## Constructors

### **RegistrationConsumerTestHarness(IConsumerFactoryDecoratorRegistration\<TConsumer\>)**

```csharp
public RegistrationConsumerTestHarness(IConsumerFactoryDecoratorRegistration<TConsumer> registration)
```

#### Parameters

`registration` [IConsumerFactoryDecoratorRegistration\<TConsumer\>](../masstransit-dependencyinjection-registration/iconsumerfactorydecoratorregistration-1)<br/>
