---

title: ISagaRepositoryDecoratorRegistration<TSaga>

---

# ISagaRepositoryDecoratorRegistration\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public interface ISagaRepositoryDecoratorRegistration<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Properties

### **TestTimeout**

```csharp
public abstract TimeSpan TestTimeout { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Consumed**

```csharp
public abstract ReceivedMessageList Consumed { get; }
```

#### Property Value

[ReceivedMessageList](../masstransit-testing/receivedmessagelist)<br/>

### **Created**

```csharp
public abstract SagaList<TSaga> Created { get; }
```

#### Property Value

[SagaList\<TSaga\>](../masstransit-testing-implementations/sagalist-1)<br/>

### **Sagas**

```csharp
public abstract SagaList<TSaga> Sagas { get; }
```

#### Property Value

[SagaList\<TSaga\>](../masstransit-testing-implementations/sagalist-1)<br/>

## Methods

### **DecorateSagaRepository(ISagaRepository\<TSaga\>)**

Decorate the container-based saga repository, returning the saga repository that should be
 used for receive endpoint registration

```csharp
ISagaRepository<TSaga> DecorateSagaRepository(ISagaRepository<TSaga> repository)
```

#### Parameters

`repository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

#### Returns

[ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>
