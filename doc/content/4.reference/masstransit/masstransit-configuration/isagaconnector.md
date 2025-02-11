---

title: ISagaConnector

---

# ISagaConnector

Namespace: MassTransit.Configuration

```csharp
public interface ISagaConnector
```

## Methods

### **CreateSagaSpecification\<T\>()**

```csharp
ISagaSpecification<T> CreateSagaSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaSpecification\<T\>](../masstransit-configuration/isagaspecification-1)<br/>

### **ConnectSaga\<T\>(IConsumePipeConnector, ISagaRepository\<T\>, ISagaSpecification\<T\>)**

```csharp
ConnectHandle ConnectSaga<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> repository, ISagaSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`repository` [ISagaRepository\<T\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`specification` [ISagaSpecification\<T\>](../masstransit-configuration/isagaspecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
