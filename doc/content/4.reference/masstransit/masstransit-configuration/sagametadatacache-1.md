---

title: SagaMetadataCache<TSaga>

---

# SagaMetadataCache\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public class SagaMetadataCache<TSaga> : ISagaMetadataCache<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaMetadataCache\<TSaga\>](../masstransit-configuration/sagametadatacache-1)<br/>
Implements [ISagaMetadataCache\<TSaga\>](../masstransit-configuration/isagametadatacache-1)

## Properties

### **InitiatedByTypes**

```csharp
public static SagaInterfaceType[] InitiatedByTypes { get; }
```

#### Property Value

[SagaInterfaceType[]](../masstransit-configuration/sagainterfacetype)<br/>

### **OrchestratesTypes**

```csharp
public static SagaInterfaceType[] OrchestratesTypes { get; }
```

#### Property Value

[SagaInterfaceType[]](../masstransit-configuration/sagainterfacetype)<br/>

### **ObservesTypes**

```csharp
public static SagaInterfaceType[] ObservesTypes { get; }
```

#### Property Value

[SagaInterfaceType[]](../masstransit-configuration/sagainterfacetype)<br/>

### **InitiatedByOrOrchestratesTypes**

```csharp
public static SagaInterfaceType[] InitiatedByOrOrchestratesTypes { get; }
```

#### Property Value

[SagaInterfaceType[]](../masstransit-configuration/sagainterfacetype)<br/>

### **FactoryMethod**

```csharp
public static SagaInstanceFactoryMethod<TSaga> FactoryMethod { get; }
```

#### Property Value

[SagaInstanceFactoryMethod\<TSaga\>](../masstransit-saga/sagainstancefactorymethod-1)<br/>
