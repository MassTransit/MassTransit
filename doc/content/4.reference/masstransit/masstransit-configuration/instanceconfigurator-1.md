---

title: InstanceConfigurator<TInstance>

---

# InstanceConfigurator\<TInstance\>

Namespace: MassTransit.Configuration

```csharp
public class InstanceConfigurator<TInstance> : IInstanceConfigurator<TInstance>, IConsumerConfigurator<TInstance>, IPipeConfigurator<ConsumerConsumeContext<TInstance>>, IConsumerConfigurator, IConsumeConfigurator, IConsumerConfigurationObserverConnector, IOptionsSet, IInstanceConfigurator, IReceiveEndpointSpecification, ISpecification
```

#### Type Parameters

`TInstance`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstanceConfigurator\<TInstance\>](../masstransit-configuration/instanceconfigurator-1)<br/>
Implements [IInstanceConfigurator\<TInstance\>](../../masstransit-abstractions/masstransit/iinstanceconfigurator-1), [IConsumerConfigurator\<TInstance\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1), [IPipeConfigurator\<ConsumerConsumeContext\<TInstance\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurator](../../masstransit-abstractions/masstransit/iconsumerconfigurator), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [IOptionsSet](../../masstransit-abstractions/masstransit-configuration/ioptionsset), [IInstanceConfigurator](../../masstransit-abstractions/masstransit/iinstanceconfigurator), [IReceiveEndpointSpecification](../../masstransit-abstractions/masstransit/ireceiveendpointspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **InstanceConfigurator(TInstance, IConsumerConfigurationObserver)**

```csharp
public InstanceConfigurator(TInstance instance, IConsumerConfigurationObserver observer)
```

#### Parameters

`instance` TInstance<br/>

`observer` [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)<br/>

## Methods

### **Message\<T\>(Action\<IConsumerMessageConfigurator\<T\>\>)**

```csharp
public void Message<T>(Action<IConsumerMessageConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConsumerMessage\<T\>(Action\<IConsumerMessageConfigurator\<TInstance, T\>\>)**

```csharp
public void ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TInstance, T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<TInstance, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Options\<T\>(Action\<T\>)**

```csharp
public T Options<T>(Action<T> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

T<br/>

### **Options\<T\>(T, Action\<T\>)**

```csharp
public T Options<T>(T options, Action<T> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`options` T<br/>

`configure` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

T<br/>

### **TryGetOptions\<T\>(T)**

```csharp
public bool TryGetOptions<T>(out T options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`options` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SelectOptions\<T\>()**

```csharp
public IEnumerable<T> SelectOptions<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<ConsumerConsumeContext\<TInstance\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TInstance>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumerConsumeContext\<TInstance\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

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
