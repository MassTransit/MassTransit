---

title: MessageEventCorrelation<TSaga, TMessage>

---

# MessageEventCorrelation\<TSaga, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class MessageEventCorrelation<TSaga, TMessage> : EventCorrelation<TSaga, TMessage>, EventCorrelation, ISpecification
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageEventCorrelation\<TSaga, TMessage\>](../masstransit-sagastatemachine/messageeventcorrelation-2)<br/>
Implements [EventCorrelation\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/eventcorrelation-2), [EventCorrelation](../../masstransit-abstractions/masstransit/eventcorrelation), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **FilterFactory**

```csharp
public SagaFilterFactory<TSaga, TMessage> FilterFactory { get; }
```

#### Property Value

[SagaFilterFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagafilterfactory-2)<br/>

### **Event**

```csharp
public Event<TMessage> Event { get; }
```

#### Property Value

[Event\<TMessage\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **DataType**

```csharp
public Type DataType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **MessageFilter**

```csharp
public IFilter<ConsumeContext<TMessage>> MessageFilter { get; }
```

#### Property Value

[IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

### **Policy**

```csharp
public ISagaPolicy<TSaga, TMessage> Policy { get; }
```

#### Property Value

[ISagaPolicy\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagapolicy-2)<br/>

## Constructors

### **MessageEventCorrelation(SagaStateMachine\<TSaga\>, Event\<TMessage\>, SagaFilterFactory\<TSaga, TMessage\>, IFilter\<ConsumeContext\<TMessage\>\>, IPipe\<ConsumeContext\<TMessage\>\>, ISagaFactory\<TSaga, TMessage\>, Boolean, Boolean, Boolean)**

```csharp
public MessageEventCorrelation(SagaStateMachine<TSaga> machine, Event<TMessage> event, SagaFilterFactory<TSaga, TMessage> sagaFilterFactory, IFilter<ConsumeContext<TMessage>> messageFilter, IPipe<ConsumeContext<TMessage>> missingPipe, ISagaFactory<TSaga, TMessage> sagaFactory, bool insertOnInitial, bool readOnly, bool configureConsumeTopology)
```

#### Parameters

`machine` [SagaStateMachine\<TSaga\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

`event` [Event\<TMessage\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`sagaFilterFactory` [SagaFilterFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagafilterfactory-2)<br/>

`messageFilter` [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

`missingPipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`sagaFactory` [ISagaFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagafactory-2)<br/>

`insertOnInitial` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`readOnly` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`configureConsumeTopology` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
