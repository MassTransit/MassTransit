---

title: MessageHandlerConsumerDefinition<TConsumer, TMessage>

---

# MessageHandlerConsumerDefinition\<TConsumer, TMessage\>

Namespace: MassTransit.DependencyInjection

```csharp
public class MessageHandlerConsumerDefinition<TConsumer, TMessage> : IConsumerDefinition<TConsumer>, IConsumerDefinition, IDefinition
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageHandlerConsumerDefinition\<TConsumer, TMessage\>](../masstransit-dependencyinjection/messagehandlerconsumerdefinition-2)<br/>
Implements [IConsumerDefinition\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerdefinition-1), [IConsumerDefinition](../../masstransit-abstractions/masstransit/iconsumerdefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition)

## Properties

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConsumerType**

```csharp
public Type ConsumerType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **EndpointDefinition**

```csharp
public IEndpointDefinition<TConsumer> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<TConsumer\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

## Constructors

### **MessageHandlerConsumerDefinition()**

```csharp
public MessageHandlerConsumerDefinition()
```

## Methods

### **Configure(IReceiveEndpointConfigurator, IConsumerConfigurator\<TConsumer\>, IRegistrationContext)**

```csharp
public void Configure(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`consumerConfigurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **GetEndpointName(IEndpointNameFormatter)**

```csharp
public string GetEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
