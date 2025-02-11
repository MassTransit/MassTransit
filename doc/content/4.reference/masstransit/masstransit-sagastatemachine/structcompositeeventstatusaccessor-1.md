---

title: StructCompositeEventStatusAccessor<TSaga>

---

# StructCompositeEventStatusAccessor\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class StructCompositeEventStatusAccessor<TSaga> : ICompositeEventStatusAccessor<TSaga>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StructCompositeEventStatusAccessor\<TSaga\>](../masstransit-sagastatemachine/structcompositeeventstatusaccessor-1)<br/>
Implements [ICompositeEventStatusAccessor\<TSaga\>](../masstransit-sagastatemachine/icompositeeventstatusaccessor-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **StructCompositeEventStatusAccessor(PropertyInfo)**

```csharp
public StructCompositeEventStatusAccessor(PropertyInfo propertyInfo)
```

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Get(TSaga)**

```csharp
public CompositeEventStatus Get(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

#### Returns

[CompositeEventStatus](../../masstransit-abstractions/masstransit/compositeeventstatus)<br/>

### **Set(TSaga, CompositeEventStatus)**

```csharp
public void Set(TSaga instance, CompositeEventStatus status)
```

#### Parameters

`instance` TSaga<br/>

`status` [CompositeEventStatus](../../masstransit-abstractions/masstransit/compositeeventstatus)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
