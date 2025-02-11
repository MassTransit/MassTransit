---

title: SagaConfigurator<TSaga>

---

# SagaConfigurator\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public class SagaConfigurator<TSaga> : ISagaConfigurator<TSaga>, IPipeConfigurator<SagaConsumeContext<TSaga>>, ISagaConfigurationObserverConnector, IConsumeConfigurator, IOptionsSet, IReceiveEndpointSpecification, ISpecification
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaConfigurator\<TSaga\>](../masstransit-configuration/sagaconfigurator-1)<br/>
Implements [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1), [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IOptionsSet](../../masstransit-abstractions/masstransit-configuration/ioptionsset), [IReceiveEndpointSpecification](../../masstransit-abstractions/masstransit/ireceiveendpointspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SagaConfigurator(ISagaRepository\<TSaga\>, ISagaConfigurationObserver)**

```csharp
public SagaConfigurator(ISagaRepository<TSaga> sagaRepository, ISagaConfigurationObserver observer)
```

#### Parameters

`sagaRepository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`observer` [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver)<br/>

## Methods

### **Configure(IReceiveEndpointBuilder)**

```csharp
public void Configure(IReceiveEndpointBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

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
