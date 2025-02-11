---

title: CorrelatedByFaultEventCorrelationBuilder<TInstance, TData>

---

# CorrelatedByFaultEventCorrelationBuilder\<TInstance, TData\>

Namespace: MassTransit.Configuration

```csharp
public class CorrelatedByFaultEventCorrelationBuilder<TInstance, TData> : IEventCorrelationBuilder
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CorrelatedByFaultEventCorrelationBuilder\<TInstance, TData\>](../masstransit-configuration/correlatedbyfaulteventcorrelationbuilder-2)<br/>
Implements [IEventCorrelationBuilder](../masstransit-configuration/ieventcorrelationbuilder)

## Constructors

### **CorrelatedByFaultEventCorrelationBuilder(SagaStateMachine\<TInstance\>, Event\<Fault\<TData\>\>)**

```csharp
public CorrelatedByFaultEventCorrelationBuilder(SagaStateMachine<TInstance> machine, Event<Fault<TData>> event)
```

#### Parameters

`machine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

`event` [Event\<Fault\<TData\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Methods

### **Build()**

```csharp
public EventCorrelation Build()
```

#### Returns

[EventCorrelation](../../masstransit-abstractions/masstransit/eventcorrelation)<br/>
