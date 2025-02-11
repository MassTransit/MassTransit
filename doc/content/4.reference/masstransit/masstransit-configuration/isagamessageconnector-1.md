---

title: ISagaMessageConnector<TSaga>

---

# ISagaMessageConnector\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public interface ISagaMessageConnector<TSaga> : ISagaMessageConnector
```

#### Type Parameters

`TSaga`<br/>

Implements [ISagaMessageConnector](../masstransit-configuration/isagamessageconnector)

## Methods

### **CreateSagaMessageSpecification()**

```csharp
ISagaMessageSpecification<TSaga> CreateSagaMessageSpecification()
```

#### Returns

[ISagaMessageSpecification\<TSaga\>](../masstransit-configuration/isagamessagespecification-1)<br/>

### **ConnectSaga(IConsumePipeConnector, ISagaRepository\<TSaga\>, ISagaSpecification\<TSaga\>)**

```csharp
ConnectHandle ConnectSaga(IConsumePipeConnector consumePipe, ISagaRepository<TSaga> repository, ISagaSpecification<TSaga> specification)
```

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`specification` [ISagaSpecification\<TSaga\>](../masstransit-configuration/isagaspecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
