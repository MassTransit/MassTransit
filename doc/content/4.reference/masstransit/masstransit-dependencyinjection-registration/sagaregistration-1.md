---

title: SagaRegistration<TSaga>

---

# SagaRegistration\<TSaga\>

Namespace: MassTransit.DependencyInjection.Registration

A saga registration represents a single saga, which will use the container for the scope provider, as well as
 to resolve the saga repository.

```csharp
public class SagaRegistration<TSaga> : ISagaRegistration, IRegistration
```

#### Type Parameters

`TSaga`<br/>
The saga type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaRegistration\<TSaga\>](../masstransit-dependencyinjection-registration/sagaregistration-1)<br/>
Implements [ISagaRegistration](../masstransit-configuration/isagaregistration), [IRegistration](../masstransit-configuration/iregistration)

## Properties

### **Type**

```csharp
public Type Type { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **IncludeInConfigureEndpoints**

```csharp
public bool IncludeInConfigureEndpoints { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **SagaRegistration(IContainerSelector)**

```csharp
public SagaRegistration(IContainerSelector selector)
```

#### Parameters

`selector` [IContainerSelector](../masstransit-configuration/icontainerselector)<br/>
