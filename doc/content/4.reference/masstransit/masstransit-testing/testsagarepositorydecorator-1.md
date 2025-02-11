---

title: TestSagaRepositoryDecorator<TSaga>

---

# TestSagaRepositoryDecorator\<TSaga\>

Namespace: MassTransit.Testing

```csharp
public class TestSagaRepositoryDecorator<TSaga> : ISagaRepository<TSaga>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TestSagaRepositoryDecorator\<TSaga\>](../masstransit-testing/testsagarepositorydecorator-1)<br/>
Implements [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **TestSagaRepositoryDecorator(ISagaRepository\<TSaga\>, ReceivedMessageList, SagaList\<TSaga\>, SagaList\<TSaga\>)**

```csharp
public TestSagaRepositoryDecorator(ISagaRepository<TSaga> sagaRepository, ReceivedMessageList received, SagaList<TSaga> created, SagaList<TSaga> sagas)
```

#### Parameters

`sagaRepository` [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`received` [ReceivedMessageList](../masstransit-testing/receivedmessagelist)<br/>

`created` [SagaList\<TSaga\>](../masstransit-testing-implementations/sagalist-1)<br/>

`sagas` [SagaList\<TSaga\>](../masstransit-testing-implementations/sagalist-1)<br/>
