---

title: DefaultSagaDefinition<TSaga>

---

# DefaultSagaDefinition\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public class DefaultSagaDefinition<TSaga> : SagaDefinition<TSaga>, ISagaDefinition<TSaga>, ISagaDefinition, IDefinition
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaDefinition\<TSaga\>](../../masstransit-abstractions/masstransit/sagadefinition-1) → [DefaultSagaDefinition\<TSaga\>](../masstransit-configuration/defaultsagadefinition-1)<br/>
Implements [ISagaDefinition\<TSaga\>](../../masstransit-abstractions/masstransit/isagadefinition-1), [ISagaDefinition](../../masstransit-abstractions/masstransit/isagadefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition)

## Properties

### **EndpointDefinition**

```csharp
public IEndpointDefinition<TSaga> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<TSaga\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **DefaultSagaDefinition()**

```csharp
public DefaultSagaDefinition()
```
