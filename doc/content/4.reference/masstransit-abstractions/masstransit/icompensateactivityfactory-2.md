---

title: ICompensateActivityFactory<TActivity, TLog>

---

# ICompensateActivityFactory\<TActivity, TLog\>

Namespace: MassTransit

```csharp
public interface ICompensateActivityFactory<TActivity, TLog> : IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Compensate(CompensateContext\<TLog\>, IPipe\<CompensateActivityContext\<TActivity, TLog\>\>)**

```csharp
Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../masstransit/compensatecontext-1)<br/>

`next` [IPipe\<CompensateActivityContext\<TActivity, TLog\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
