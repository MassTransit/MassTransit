---

title: DefaultConsumerDefinition<TConsumer>

---

# DefaultConsumerDefinition\<TConsumer\>

Namespace: MassTransit.Configuration

A default consumer definition, used if no definition is found for the consumer type

```csharp
public class DefaultConsumerDefinition<TConsumer> : ConsumerDefinition<TConsumer>, IConsumerDefinition<TConsumer>, IConsumerDefinition, IDefinition
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerDefinition\<TConsumer\>](../../masstransit-abstractions/masstransit/consumerdefinition-1) → [DefaultConsumerDefinition\<TConsumer\>](../masstransit-configuration/defaultconsumerdefinition-1)<br/>
Implements [IConsumerDefinition\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerdefinition-1), [IConsumerDefinition](../../masstransit-abstractions/masstransit/iconsumerdefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition)

## Properties

### **EndpointDefinition**

```csharp
public IEndpointDefinition<TConsumer> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<TConsumer\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **DefaultConsumerDefinition()**

```csharp
public DefaultConsumerDefinition()
```
