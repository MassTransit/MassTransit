---

title: ISagaMetadataCache<TSaga>

---

# ISagaMetadataCache\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public interface ISagaMetadataCache<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Properties

### **InitiatedByTypes**

```csharp
public abstract SagaInterfaceType[] InitiatedByTypes { get; }
```

#### Property Value

[SagaInterfaceType[]](../masstransit-configuration/sagainterfacetype)<br/>

### **OrchestratesTypes**

```csharp
public abstract SagaInterfaceType[] OrchestratesTypes { get; }
```

#### Property Value

[SagaInterfaceType[]](../masstransit-configuration/sagainterfacetype)<br/>

### **ObservesTypes**

```csharp
public abstract SagaInterfaceType[] ObservesTypes { get; }
```

#### Property Value

[SagaInterfaceType[]](../masstransit-configuration/sagainterfacetype)<br/>

### **InitiatedByOrOrchestratesTypes**

```csharp
public abstract SagaInterfaceType[] InitiatedByOrOrchestratesTypes { get; }
```

#### Property Value

[SagaInterfaceType[]](../masstransit-configuration/sagainterfacetype)<br/>

### **FactoryMethod**

```csharp
public abstract SagaInstanceFactoryMethod<TSaga> FactoryMethod { get; }
```

#### Property Value

[SagaInstanceFactoryMethod\<TSaga\>](../masstransit-saga/sagainstancefactorymethod-1)<br/>
