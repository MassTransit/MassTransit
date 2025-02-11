---

title: ConsumerFactoryConfiguratorExtensions

---

# ConsumerFactoryConfiguratorExtensions

Namespace: MassTransit.Configuration

```csharp
public static class ConsumerFactoryConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerFactoryConfiguratorExtensions](../masstransit-configuration/consumerfactoryconfiguratorextensions)

## Methods

### **ValidateConsumer\<TConsumer\>(ISpecification)**

```csharp
public static IEnumerable<ValidationResult> ValidateConsumer<TConsumer>(ISpecification configurator)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [ISpecification](../../masstransit-abstractions/masstransit/ispecification)<br/>

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Validate\<TConsumer\>(IConsumerFactory\<TConsumer\>)**

```csharp
public static IEnumerable<ValidationResult> Validate<TConsumer>(IConsumerFactory<TConsumer> consumerFactory)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
