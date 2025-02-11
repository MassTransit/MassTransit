---

title: UntypedConsumerConfigurator<TConsumer>

---

# UntypedConsumerConfigurator\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public class UntypedConsumerConfigurator<TConsumer> : IConsumerConfigurator, IConsumeConfigurator, IConsumerConfigurationObserverConnector, IReceiveEndpointSpecification, ISpecification
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [UntypedConsumerConfigurator\<TConsumer\>](../masstransit-configuration/untypedconsumerconfigurator-1)<br/>
Implements [IConsumerConfigurator](../../masstransit-abstractions/masstransit/iconsumerconfigurator), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [IReceiveEndpointSpecification](../../masstransit-abstractions/masstransit/ireceiveendpointspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **UntypedConsumerConfigurator(Func\<Type, Object\>, IConsumerConfigurationObserver)**

```csharp
public UntypedConsumerConfigurator(Func<Type, object> consumerFactory, IConsumerConfigurationObserver observer)
```

#### Parameters

`consumerFactory` [Func\<Type, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`observer` [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)<br/>

## Methods

### **ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver)**

```csharp
public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
```

#### Parameters

`observer` [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Configure(IReceiveEndpointBuilder)**

```csharp
public void Configure(IReceiveEndpointBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder)<br/>
