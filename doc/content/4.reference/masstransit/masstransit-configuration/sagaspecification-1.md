---

title: SagaSpecification<TSaga>

---

# SagaSpecification\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public class SagaSpecification<TSaga> : OptionsSet, IOptionsSet, ISagaSpecification<TSaga>, ISagaConfigurator<TSaga>, IPipeConfigurator<SagaConsumeContext<TSaga>>, ISagaConfigurationObserverConnector, IConsumeConfigurator, ISpecification
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OptionsSet](../../masstransit-abstractions/masstransit-configuration/optionsset) → [SagaSpecification\<TSaga\>](../masstransit-configuration/sagaspecification-1)<br/>
Implements [IOptionsSet](../../masstransit-abstractions/masstransit-configuration/ioptionsset), [ISagaSpecification\<TSaga\>](../masstransit-configuration/isagaspecification-1), [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1), [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SagaSpecification(IEnumerable\<ISagaMessageSpecification\<TSaga\>\>)**

```csharp
public SagaSpecification(IEnumerable<ISagaMessageSpecification<TSaga>> messageSpecifications)
```

#### Parameters

`messageSpecifications` [IEnumerable\<ISagaMessageSpecification\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **Message\<T\>(Action\<ISagaMessageConfigurator\<T\>\>)**

```csharp
public void Message<T>(Action<ISagaMessageConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<ISagaMessageConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **SagaMessage\<T\>(Action\<ISagaMessageConfigurator\<TSaga, T\>\>)**

```csharp
public void SagaMessage<T>(Action<ISagaMessageConfigurator<TSaga, T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<ISagaMessageConfigurator\<TSaga, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **GetMessageSpecification\<T\>()**

```csharp
public ISagaMessageSpecification<TSaga, T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaMessageSpecification\<TSaga, T\>](../masstransit-configuration/isagamessagespecification-2)<br/>

### **ConfigureMessagePipe\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>)**

```csharp
public void ConfigureMessagePipe<T>(IPipeConfigurator<ConsumeContext<T>> pipeConfigurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipeConfigurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<SagaConsumeContext\<TSaga\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **ConnectSagaConfigurationObserver(ISagaConfigurationObserver)**

```csharp
public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
```

#### Parameters

`observer` [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
