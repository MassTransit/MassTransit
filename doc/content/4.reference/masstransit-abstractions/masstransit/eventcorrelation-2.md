---

title: EventCorrelation<TInstance, TData>

---

# EventCorrelation\<TInstance, TData\>

Namespace: MassTransit

```csharp
public interface EventCorrelation<TInstance, TData> : EventCorrelation, ISpecification
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Implements [EventCorrelation](../masstransit/eventcorrelation), [ISpecification](../masstransit/ispecification)

## Properties

### **Event**

```csharp
public abstract Event<TData> Event { get; }
```

#### Property Value

[Event\<TData\>](../masstransit/event-1)<br/>

### **Policy**

Returns the saga policy for the event correlation

```csharp
public abstract ISagaPolicy<TInstance, TData> Policy { get; }
```

#### Property Value

[ISagaPolicy\<TInstance, TData\>](../masstransit/isagapolicy-2)<br/>

### **FilterFactory**

The filter factory creates the filter when requested by the connector

```csharp
public abstract SagaFilterFactory<TInstance, TData> FilterFactory { get; }
```

#### Property Value

[SagaFilterFactory\<TInstance, TData\>](../masstransit/sagafilterfactory-2)<br/>

### **MessageFilter**

The message filter which extracts the correlationId from the message

```csharp
public abstract IFilter<ConsumeContext<TData>> MessageFilter { get; }
```

#### Property Value

[IFilter\<ConsumeContext\<TData\>\>](../masstransit/ifilter-1)<br/>
