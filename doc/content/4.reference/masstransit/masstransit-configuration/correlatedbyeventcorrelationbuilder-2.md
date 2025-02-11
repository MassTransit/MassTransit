---

title: CorrelatedByEventCorrelationBuilder<TInstance, TData>

---

# CorrelatedByEventCorrelationBuilder\<TInstance, TData\>

Namespace: MassTransit.Configuration

```csharp
public class CorrelatedByEventCorrelationBuilder<TInstance, TData> : IEventCorrelationBuilder
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CorrelatedByEventCorrelationBuilder\<TInstance, TData\>](../masstransit-configuration/correlatedbyeventcorrelationbuilder-2)<br/>
Implements [IEventCorrelationBuilder](../masstransit-configuration/ieventcorrelationbuilder)

## Constructors

### **CorrelatedByEventCorrelationBuilder(SagaStateMachine\<TInstance\>, Event\<TData\>)**

```csharp
public CorrelatedByEventCorrelationBuilder(SagaStateMachine<TInstance> machine, Event<TData> event)
```

#### Parameters

`machine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Methods

### **Build()**

```csharp
public EventCorrelation Build()
```

#### Returns

[EventCorrelation](../../masstransit-abstractions/masstransit/eventcorrelation)<br/>
