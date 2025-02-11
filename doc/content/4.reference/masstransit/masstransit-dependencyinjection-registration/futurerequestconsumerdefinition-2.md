---

title: FutureRequestConsumerDefinition<TConsumer, TRequest>

---

# FutureRequestConsumerDefinition\<TConsumer, TRequest\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class FutureRequestConsumerDefinition<TConsumer, TRequest> : ConsumerDefinition<TConsumer>, IConsumerDefinition<TConsumer>, IConsumerDefinition, IDefinition, IFutureRequestDefinition<TRequest>
```

#### Type Parameters

`TConsumer`<br/>

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerDefinition\<TConsumer\>](../../masstransit-abstractions/masstransit/consumerdefinition-1) → [FutureRequestConsumerDefinition\<TConsumer, TRequest\>](../masstransit-dependencyinjection-registration/futurerequestconsumerdefinition-2)<br/>
Implements [IConsumerDefinition\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerdefinition-1), [IConsumerDefinition](../../masstransit-abstractions/masstransit/iconsumerdefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition), [IFutureRequestDefinition\<TRequest\>](../masstransit/ifuturerequestdefinition-1)

## Properties

### **RequestAddress**

```csharp
public Uri RequestAddress { get; }
```

#### Property Value

Uri<br/>

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

### **FutureRequestConsumerDefinition()**

```csharp
public FutureRequestConsumerDefinition()
```

## Methods

### **ConfigureConsumer(IReceiveEndpointConfigurator, IConsumerConfigurator\<TConsumer\>, IRegistrationContext)**

```csharp
protected void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator, IRegistrationContext context)
```

#### Parameters

`endpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`consumerConfigurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
